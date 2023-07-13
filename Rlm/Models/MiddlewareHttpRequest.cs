using System.Net;
using Microsoft.AspNetCore.Http;
using Rlm.Extensions;
using Rlm.Models.Enums;

namespace Rlm.Models;

public class MiddlewareHttpRequest
{
    public string Method { get; set; }
    public string Path { get; set; }
    public IPAddress? RemoteIpv4 { get; set; }
    public IPAddress? RemoteIpv6 { get; set; }
    public QueryString Query { get; set; }
    public IHeaderDictionary Headers { get; set; }
    public string Scheme { get; set; }
    public HostString Host { get; set; }
    public string Body { get; set; }
    
    public Dictionary<string, string> CustomProperties { get; set; } = new();

    public override string ToString() {
        var msg = $"HTTP request information:\n" +
                  $"{CustomProperties.FormatToLog()}" +
                  $"\tMethod: {Method}\n" +
                  $"\tPath: {Path}\n" +
                  $"\tRemoteIpv4: {RemoteIpv4}\n" +
                  $"\tRemoteIpv6: {RemoteIpv6}\n" +
                  $"\tQueryString: {Query}\n" +
                  $"\tHeaders: {DictionaryExtensions.FormatHeaders(Headers)}\n" +
                  $"\tSchema: {Scheme}\n" +
                  $"\tHost: {Host}\n" +
                  $"\tBody: {Body}";

        return msg;
    }

    public void Log(LogSkippedReason? skippedReason = null) {
        if (skippedReason != null) {
            Serilog.Log.Debug("Skipped Logging cause: {Reason}", skippedReason.ToString());
            Serilog.Log.Debug(this.ToString());    
        } else {
            Serilog.Log.Information(this.ToString());
        }
    }
}