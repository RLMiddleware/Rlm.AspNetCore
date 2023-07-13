using System.Net;
using Microsoft.AspNetCore.Http;
using Rlm.Extensions;
using Rlm.Models.Enums;

namespace Rlm.Models;

public class MiddlewareHttpResponse
{
    public int StatusCode { get; set; }
    public string? ContentType { get; set; }
    public IPAddress? RemoteIpv4 { get; set; }
    public IPAddress? RemoveIpv6 { get; set; }
    public IHeaderDictionary Headers { get; set; }
    public string Body { get; set; }

    public Dictionary<string, string> CustomProperties { get; set; } = new();

    public override string ToString() {
        var msg = $"HTTP response information:\n" +
                  $"{CustomProperties.FormatToLog()}" +
                  $"\tStatusCode: {StatusCode}\n" +
                  $"\tContentType: {ContentType}\n" +
                  $"\tRemoteIpv4: {RemoteIpv4}\n" +
                  $"\tRemoteIpv6: {RemoveIpv6}\n" +
                  $"\tHeaders: {DictionaryExtensions.FormatHeaders(Headers)}\n" +
                  $"\tBody: {Body}";
        return msg;
    }


    public void Log(LogSkippedReason? skippedReason = null) {
        if (skippedReason != null) {
            Serilog.Log.Debug("Skipped Logging cause: {Reason}", skippedReason.ToString());
            Serilog.Log.Debug(this.ToString());
        } else {
            if (this.StatusCode is < 200 or >= 300) {
                Serilog.Log.Error("{Log}", this.ToString());
            } else {
                Serilog.Log.Information("{Log}", this.ToString());
            }
        }
    }
}