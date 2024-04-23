using System.Text.Json.Serialization;
using headers.security.Api;
using headers.security.Api.Middlewares;
using headers.security.Api.Services;
using headers.security.Api.Extensions;
using headers.security.Scanner;
using headers.security.Scanner.Hsts;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(builder.Configuration
    .GetSection("WorkerOptions")
    .Get<WorkerConfiguration>()
);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<Crawler>();
builder.Services.AddSingleton<IHstsPreloadRepository, CachingHstsPreloadRepository>();
builder.Services.AddSingleton<HstsPreloadClient>();
builder.Services.AddSingleton<HstsPreloadService>();
builder.Services.AddSingleton<Worker>();
builder.Services.AddHostedService<HstsPreloadUpdaterBackgroundService>();
builder.Services.AddSecurityEngine();

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

builder.Services.AddHttpClient("Scanner", HttpClientHelper.ConfigureClient)
    .ConfigurePrimaryHttpMessageHandler(HttpClientHelper.ConfigureHandler);
builder.Services.AddHttpClient("Integration");

builder.Services.AddHsts(options =>
{
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddRateLimiter(RateLimiterFactory.ConfigureOptions);

var app = builder.Build();

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

if (app.Environment.IsProduction())
{
    app.UseRateLimiter();
}

app.MapControllers().RequireRateLimiting(RateLimiterFactory.PolicyName);
app.UseSpa(_ => { });

app.Run();