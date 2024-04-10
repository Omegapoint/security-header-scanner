using System.Text.Json.Serialization;
using headers.security.Api.Middlewares;
using headers.security.Scanner;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(builder.Configuration
    .GetSection("WorkerOptions")
    .Get<WorkerConfiguration>()
);

builder.Services.AddSingleton<ICrawler, Crawler>();
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

builder.Services.AddHttpClient("Scanner", HttpClientHelper.ConfigureClient)
    .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
    {
        AllowAutoRedirect = false
    });

builder.Services.AddHsts(options =>
{
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddRateLimiter(RateLimiterFactory.ConfigureOptions);

var app = builder.Build();

app.UseAppIdentification();
app.DenySelfRequests();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseFileServer();
app.UseRouting();

if (app.Environment.IsProduction())
{
    app.UseRateLimiter();
}

app.MapControllers().RequireRateLimiting(RateLimiterFactory.PolicyName);
app.UseSpa(_ => { });

app.Run();