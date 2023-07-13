using Microsoft.AspNetCore.Http;
using Rlm.Models;
using Rlm.Utils;

namespace Rlm.Extensions;

public static class HttpExtensions
{
    public static async Task<MiddlewareHttpResponse> GetResponse(this HttpContext context, RlmSettings rlmSettings) {
        if (context.Items.TryGetValue(PublicConstants.HttpResponsePlaceholder, out var item)) {
            return item as MiddlewareHttpResponse;
        }
        
        var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var remoteIp = context.Connection.RemoteIpAddress;
        return new MiddlewareHttpResponse {
            CustomProperties = rlmSettings.CustomResponseProperties.Invoke(context),
            StatusCode = context.Response.StatusCode,
            ContentType = context.Response.ContentType,
            RemoteIpv4 = remoteIp?.MapToIPv4(),
            RemoveIpv6 = remoteIp?.MapToIPv6(),
            Headers = context.Response.Headers,
            Body = HelperMethods.FormatBody(responseBodyText, rlmSettings),
        };
    }

    public static async Task<MiddlewareHttpRequest?> GetRequest(this HttpContext context, RlmSettings config) {
        if (context.Items.TryGetValue(PublicConstants.HttpRequestPlaceholder, out var item)) {
            return item as MiddlewareHttpRequest;
        }

        var remoteIp = context.Connection.RemoteIpAddress;

        return new MiddlewareHttpRequest {
            CustomProperties = config.CustomRequestProperties.Invoke(context),
            Method = context.Request.Method,
            Path = context.Request.Path,
            RemoteIpv4 = remoteIp?.MapToIPv4(),
            RemoteIpv6 = remoteIp?.MapToIPv6(),
            Query = context.Request.QueryString,
            Headers = context.Request.Headers,
            Scheme = context.Request.Scheme,
            Host = context.Request.Host,
            Body = await ReadBodyFromRequest(context, config),
        };
    }


    private static async Task<string> ReadBodyFromRequest(HttpContext context, RlmSettings config) {
        if (context.Request.ContentLength > config.MaxRequestLengthKb) {
            return "";
        }
        
        var request = context.Request;
        // Ensure the request's body can be read multiple times (for the next middlewares in the pipeline).
        // request.EnableBuffering();
        request.Body.Position = 0;

        using var streamReader = new StreamReader(request.Body, leaveOpen: true);
        var requestBody = await streamReader.ReadToEndAsync();

        // Reset the request's body stream position for next middleware in the pipeline.
        request.Body.Position = 0;

        return HelperMethods.FormatBody(requestBody, config);
    }
}