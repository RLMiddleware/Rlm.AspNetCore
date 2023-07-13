/* Copyright (c) Wesche-IT
 * ALL RIGHTS RESERVED
 * This software is protected by the inclusion of the above copyright
 * notice. This software may not be provided or otherwise made available
 * to, or used by, any other person. No title to or ownership of the
 * software is  hereby  transferred.
 * The information contained in this document is considered the
 * CONFIDENTIAL and PROPRIETARY information of Wesche-IT and may
 * not be disclosed or discussed with anyone who is not employed by
 * Wesche-IT, unless the individual/company
 * (i) has an express need to know such information, and
 * (ii) disclosure of information is subject to the terms of a duly
 * executed Confidentiality and Non-Disclosure Agreement between
 * Wesche-IT and the individual/company.
 */

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Rlm.Extensions;
using Rlm.Models;
using Rlm.Models.Enums;

namespace Rlm.Middleware
{
    public class RlmMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RlmSettings _config;

        public RlmMiddleware(RequestDelegate next, RlmSettings options) {
            _next = next;
            _config = options;
        }

        public async Task InvokeAsync(HttpContext context) {
            var request = await context.GetRequest(_config);
            
            
            // Checks if path matches endpoint pattern
            if (!(_config.EndpointPattern != null && new Regex(_config.EndpointPattern).IsMatch(context.Request.Path))) {
                request?.Log(LogSkippedReason.EndpointPattern);
                await _next.Invoke(context);
                return;
            }

            // Checks if path matches excluded routes - if so, skip logging
            if (_config.ExcludedRoutes.Any(requestLoggingExcludedRoute => context.Request.Path.StartsWithSegments(requestLoggingExcludedRoute))) {
                request?.Log(LogSkippedReason.ExcludedRoutes);
                await _next.Invoke(context);
                return;
            }

            if (!_config.ContentTypes.Contains(context.Request.ContentType!)) {
                request?.Log(LogSkippedReason.ContentType);
                // TODO: log request as debug
                await _next.Invoke(context);
                return;
            }
            
            context.Items[PublicConstants.HttpRequestPlaceholder] = request;
            request?.Log();

            // Temporarily replace the HttpResponseStream, which is a write-only stream, with a MemoryStream to capture it's value in-flight.
            var originalResponseBody = context.Response.Body;
            using var newResponseBody = new MemoryStream();
            context.Response.Body = newResponseBody;

            // Call the next middleware in the pipeline
            await _next(context);

            newResponseBody.Seek(0, SeekOrigin.Begin);

            var response = await context.GetResponse(_config);
            context.Items[PublicConstants.HttpResponsePlaceholder] = response;
            response.Log();

            newResponseBody.Seek(0, SeekOrigin.Begin);
            await newResponseBody.CopyToAsync(originalResponseBody);
        }
    }
}