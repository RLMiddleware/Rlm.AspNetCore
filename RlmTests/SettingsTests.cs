using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rlm.Extensions;
using Rlm.Models;
using Rlm.Models.Enums;
using Xunit;

namespace RlmTests;


public class SettingsTests
{
    [Fact]
    public void DefaultSettings() {
        var rlmSettings = new RlmSettings();
        Assert.Equal(PublicConstants.ApiVersionPattern, rlmSettings.EndpointPattern);
        Assert.Empty(rlmSettings.ExcludedRoutes);
        Assert.True(rlmSettings.ObscureCredentials);
        Assert.Equal(LoggingProperties.All, rlmSettings.ResponseLoggingProperties);
        Assert.Equal(LoggingProperties.All, rlmSettings.RequestLoggingProperties);
    }

    private WebApplication RegisterMiddleware() {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddRlm();
        var app = builder.Build();
        app.UseRlm();
        return app;
    }

    [Fact]
    public void MiddlewareRegistrationInit() {

        var app = RegisterMiddleware();
        var defaultSettings = new RlmSettings();
        
        // verifies that settings are registered as IOptions singleton and they are equal to default settings 
        var serviceSettings = app.Services.GetRequiredService<IOptions<RlmSettings>>();
        defaultSettings.Should().BeEquivalentTo(serviceSettings.Value);
    }

    [Fact]
    public void MiddlewareSettingsDelegate() {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddRlm(options => {
            options.ObscureCredentials = false;
            options.EndpointPattern = @"((\/app\/)(v\d))(/(.+))";
            options.ExcludedRoutes = new List<PathString> {
                "/hangfire/*",
                "/swagger/*"
            };
        });
        var app = builder.Build();
        app.UseRlm();
        
        var settings = app.Services.GetRequiredService<RlmSettings>();
        Assert.False(settings.ObscureCredentials);
        Assert.Equal(@"((\/app\/)(v\d))(/(.+))", settings.EndpointPattern);
        Assert.Equivalent(new List<PathString> {
            "/hangfire/*",
            "/swagger/*"
        }, settings.ExcludedRoutes);

        Assert.Contains("application/json", settings.ContentTypes);
    }
}