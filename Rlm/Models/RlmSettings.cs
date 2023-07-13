using Microsoft.AspNetCore.Http;
using Rlm.Models.Enums;

namespace Rlm.Models;

public class RlmSettings
{
    /**
     * Regex pattern which shall be used to track requests or leave them  
     */
    public string? EndpointPattern { get; set; } = PublicConstants.ApiVersionPattern;

    /**
     * Settings if credentials shall be obscured or not.
     */
    public bool ObscureCredentials { get; set; } = true;

    /**
     * Determine api routes which shall be excluded from logging middleware. Default is empty list
     */
    public IEnumerable<PathString> ExcludedRoutes { get; set; } = new List<PathString>();
    

    /**
     * Sets all properties which shall be logged from middleware.
     * Custom properties which are not included in LoggingProperties must be defined in custom properties
     * 
     */
    public int RequestLoggingProperties = LoggingProperties.All;

    /**
     * Sets all request properties which shall be logged from middleware.
     * Custom properties which are not included in LoggingProperties must be defined in custom properties
     * 
     */
    public int ResponseLoggingProperties = LoggingProperties.All;
    
    
    /**
     * Whitelists all request and response content types which shall be logged 
     * If content type is not in this list, request is only logged if debug is enabled
     * 
     */
    public List<string> ContentTypes = new() {
        "application/json"
    };
    
    
    /**
     * List of all json-keys of which json values shall be obscured. Currently this only affects json
     * requests & responses. Support for more content types will be added soon  
     * 
     */
    public List<string> ObscureKeywords = new() {
     "password",
     "token",
     "firebaseDeviceToken"
    };

    /**
     * Additional request properties which shall be logged besides the default values.
     * Usage:
     * builder.Services.AddRlm(options => {
     *   options.CustomRequestProperties = new Dictionary<string, Func<HttpContext, string>> {
     *       { "Protocol", context => context.Request.Protocol}}
     *   };
     * }
     * or:
     * 
     * builder.Services.AddRlm(options => {
     *   options.CustomRequestProperties.Add("Protocol", (context) => context.Request.Protocol);
     * });
     * 
     */
    public Dictionary<string, Func<HttpContext, string>> CustomRequestProperties { get; set; } = new();
    
    /**
     * Additional response properties which shall be logged besides the default values.
     * Usage:
     * builder.Services.AddRlm(options => {
     *   options.CustomResponseProperties = new Dictionary<string, Func<HttpContext, string>> {
     *       { "StatusCode", context => context.Response.StatusCode}}
     *   };
     * }
     * or:
     * 
     * builder.Services.AddRlm(options => {
     *   options.CustomRequestProperties.Add("StatusCode", (context) => context.Response.StatusCode);
     * });
     * 
     */
    public Dictionary<string, Func<HttpContext, string>> CustomResponseProperties { get; set; } = new();
    
    /**
     * Maximum request Content-Length which shall be logged. If request is bigger then maximum, request will not be logged 
     */
    public int MaxRequestLengthKb { get; set; } = 5 * 1000;
    
    /**
     * Maximum response Content-Length which shall be logged. If response is bigger then maximum, response will not be logged 
     */
    public int MaxResponseLengthKb { get; set; } = 5 * 1000;
}