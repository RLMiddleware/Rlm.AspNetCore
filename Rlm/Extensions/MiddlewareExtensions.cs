using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rlm.Middleware;
using Rlm.Models;
using Serilog;
using Serilog.Filters;

namespace Rlm.Extensions;

public static class MiddlewareExtensions
{
    public static LoggerConfiguration EnrichFromRlm(this LoggerConfiguration config, Action<LoggerConfiguration> options) {
        var settings = config.WriteTo.Logger(lc =>
            lc.Filter.ByIncludingOnly(Matching.FromSource(nameof(Middleware)))
        );
        options.Invoke(settings);
        return settings;
    }

    public static void AddRlm(this IServiceCollection services, Action<RlmSettings>? setupAction = null) {
        var settings = new RlmSettings();
        setupAction?.Invoke(settings);
        services.AddSingleton(settings);
    }

    public static void UseRlm(this IApplicationBuilder app) {
        app.UseMiddleware<RlmMiddleware>();
    }
    
}