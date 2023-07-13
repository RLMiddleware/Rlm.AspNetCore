# RLM - Request logging Middleware for Asp .Net Core

The Request Logging Middleware is a powerful library designed specifically for ASP.NET Core applications,
primarily aimed at Rest APIs. This middleware acts as a request logger,
allowing you to capture and store important information about incoming requests and responses.

This package is currently <strong>UNDER DEVELOPMENT !</strong>

## Features
- **Wildcard Support**:
  The Request Logging Middleware offers wildcard support, allowing you to specify patterns for request
  paths or endpoints. With this feature, you can capture and log requests that match a specific pattern
  or contain certain keywords. It provides flexibility in logging only the requests you're interested in  
  while excluding others.

- **Api-Path Exclusions**:
  Sometimes, you may have sensitive endpoints or paths that you don't want to log.
  The Request Logging Middleware enables you to define exclusion rules for specific API paths.
  By excluding these paths from logging, you can ensure that sensitive information remains secure
  and confidential.

- **Obscure Credentials**:
  When logging requests, it's crucial to protect sensitive information like credentials or personally
  identifiable information (PII). The Request Logging Middleware includes a built-in mechanism
  to obscure credentials in the logs. This feature ensures that sensitive data is not exposed in the
  log entries, maintaining the security and privacy of your users' information.

- **Content-Type Whitelist**:
  The Request Logging Middleware allows you to define a whitelist of content types for logging purposes.
  By specifying a content type whitelist, you can limit the logging of requests and responses
  to only those with specific content types. This feature provides fine-grained control over
  what types of content are logged, enhancing the efficiency and relevancy of your log entries.

- **Request & Response MaxLength**:
  To avoid excessively long log entries, the Request Logging Middleware offers the ability to set maximum
  lengths for both request and response bodies. This feature ensures that only a specified portion of the
  request and response payloads is included in the logs, making them more manageable and readable.

- **Custom Property Logging**:
  The Request Logging Middleware supports custom property logging, allowing you to include additional
  contextual information in your log entries. You can add custom properties to log alongside the default
  request and response information.

- **Integration with Serilog**:
  Leveraging the popular Serilog library, the Request Logging Middleware seamlessly integrates with
  your existing Serilog configuration, ensuring consistent and centralized log management.
  We provide a Serilog extension specifically tailored for this middleware,
  simplifying the setup and configuration process for your ASP.NET Core application.

## Why Use the Request Logging Middleware?

Logging is an essential aspect of any application, especially when it comes to Rest APIs.
The Request Logging Middleware helps you gain valuable <strong>insights</strong> into your API's behavior,
performance, and potential issues. By capturing request and response information,
you can effectively monitor and troubleshoot your API, improving its reliability and enhancing
the overall user experience.
<strong>Unlike the usual request logging request frameworks</strong>, the middlewares are often only able to log
a certain part of the requests or give too much output. With this library you have the advantage
of logging the request components you select or logging Api specific components.
For a better overview, you are also able to pipe the output of RLM into <strong>individual log files</strong>.

## Getting Started
Install the NuGet package: `Install-Package Rlm.AspNetCore`

### Example Usage:
1. Configure the middleware in your `Startup.cs` file:
   ```csharp
   public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
   {
       // Other middleware configurations...
    
       // Add the Request Logging Middleware
       app.AddRlm(options => {
        options.EndpointPattern = PublicConstants.ApiVersionPattern;
        options.ObscureCredentials = true;
        options.MaxRequestLengthKb = 5000;  // represents 5Mb 
        options.MaxResponseLengthKb = 5000; // represents 5Mb 
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
   
        // Other middleware configurations...
    }
       
2. Serilog Configuration:
```csharp

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .EnrichFromRlm(log =>
        log.WriteTo.File(
            path: "/var/log/my_project/my_project_webhook.log",
            rollingInterval: RollingInterval.Day
        )
    )
    .CreateLogger();
```