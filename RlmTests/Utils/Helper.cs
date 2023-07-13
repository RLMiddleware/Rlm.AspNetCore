using Microsoft.AspNetCore.Builder;
using Rlm.Extensions;

namespace RlmTests.Utils;

public class Helper
{
    public static WebApplication SetupHost(Action<WebApplicationBuilder>? additionalFeatures = null) {
        var builder = WebApplication.CreateBuilder();
        additionalFeatures?.Invoke(builder);
        var app = builder.Build();
        app.UseRlm();

        return app;
    }
}