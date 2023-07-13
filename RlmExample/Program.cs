using Rlm.Extensions;
using Rlm.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .EnrichFromRlm(log =>
        log.WriteTo.File(
            path: "Logs/webhook.log",
            rollingInterval: RollingInterval.Day
        )
    )
    .CreateLogger();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRlm(options => {
        options.EndpointPattern = PublicConstants.ApiVersionPattern;
        options.ObscureCredentials = true;
        options.MaxRequestLengthKb = 100;
        options.MaxResponseLengthKb = 100;
        options.CustomRequestProperties = new Dictionary<string, Func<HttpContext, string>> {
            { "UserId", context => context.User.Claims.Any() ? context.User.Claims.Single(x => x.Type == "user_id").Value : "" },
        };
        options.ContentTypes = new List<string> {
            "multipart/form-data",
            "application/json",
        };
        options.ExcludedRoutes = new List<PathString> {
            "/metrics",
            "/signalrServer**",
            "/hangfire",
            "/api/docs",
            "/api/swagger"
        };
        options.ObscureKeywords = new List<string> {
            "password",
            "token",
            "firebaseDeviceToken"
        };
    }
);


var app = builder.Build();


app.UseRlm();

app.UseAuthorization();
app.MapControllers();
app.MapGet("api/v0/myendpoint", () =>
    Results.Json(new { word = "Hello World!" })
);

app.Run();