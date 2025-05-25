using System.Text.Json.Serialization;
using headers.security.Api.Middlewares;
using headers.security.CachedContent.Extensions;
using headers.security.Scanner;
using headers.security.Scanner.Configuration;
using headers.security.Scanner.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(builder.Configuration
    .GetSection("WorkerOptions")
    .Get<WorkerConfiguration>()
);

var httpClientConfiguration = builder.Configuration
    .GetSection("HttpClientOptions")
    .Get<HttpClientConfiguration>();
builder.Services.AddSingleton(httpClientConfiguration);

builder.Services.AddHttpClient("Scanner", client => HttpClientHelper.ConfigureClient(client, httpClientConfiguration))
    .ConfigurePrimaryHttpMessageHandler(HttpClientHelper.ConfigureHandler);

builder.Services.AddMemoryCache();
builder.Services.AddCachedContent();
builder.Services.AddSecurityEngine();
builder.Services.AddHealthChecks();

builder.Services.AddSingleton<Crawler>();
builder.Services.AddSingleton<Worker>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo
{
    Title = "headers.security API - V1",
    Version = "v1"
}));

var allowAllCorsPolicy = new CorsPolicyBuilder()
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
    .Build();

builder.Services.AddCors(options => options.AddDefaultPolicy(allowAllCorsPolicy));

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | 
        ForwardedHeaders.XForwardedHost | 
        ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddHsts(options =>
{
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddRateLimiter(RateLimiterFactory.ConfigureOptions);

var app = builder.Build();

app.UseForwardedHeaders();

app.UseAppIdentification();
app.UseSecurityHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.DenySelfRequests();

app.UseFileServer();
app.UseRouting();
app.UseCors();

if (app.Environment.IsProduction())
{
    app.UseRateLimiter();
}

app.MapControllers().RequireRateLimiting(RateLimiterFactory.PolicyName);
app.MapHealthChecks("/healthz");
app.UseSpa(_ => { });

app.Run();