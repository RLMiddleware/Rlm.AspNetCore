using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rlm.Extensions;
using Rlm.Models;
using RlmTests.Utils;
using Xunit;

namespace RlmTests;

public class MiddlewareExtensionTests
{

    [Fact]
    public async Task ReadRequest() {

        var app = Helper.SetupHost();
        var settings = app.Services.GetRequiredService<IOptions<RlmSettings>>().Value;
        
        var context = new DefaultHttpContext {
            Connection = {
                RemoteIpAddress = IPAddress.Loopback,
                LocalPort = 5000
            }
        };
        var exampleReq = new {
            Name = "Test",
            Value = 123123123,
        };
        var jsonPayload = JsonConvert.SerializeObject(exampleReq);
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonPayload));
        context.Request.Body = stream;
        context.Request.ContentLength = stream.Length;
        context.Request.Scheme = "http";
        context.Request.Host = new HostString(IPAddress.Loopback.ToString(), 4000); 

        var req = await context.GetRequest(settings);
        
        Assert.NotNull(req);
        Assert.Equal("http", req.Scheme);
        Assert.Equal("127.0.0.1:4000", req.Host.Value);
        Assert.Equal(4000, req.Host.Port);
        Assert.Equal("127.0.0.1", req.Host.Host);
        
        Assert.NotNull(req.RemoteIpv4);
        Assert.Equal("127.0.0.1", req.RemoteIpv4.ToString());
        Assert.NotNull(req.RemoteIpv6);
        Assert.Equal("::ffff:127.0.0.1", req.RemoteIpv6.ToString());
        
        Assert.NotNull(req.Body);

    }
}