using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using headers.security.Api.Middlewares;
using headers.security.Scanner;
using Microsoft.AspNetCore.RateLimiting;
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

builder.Services.AddRateLimiter(rOptions => rOptions
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(12);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));

var app = builder.Build();

app.DenySelfRequests();
app.UseRateLimiter();

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

app.MapControllers().RequireRateLimiting("fixed");

app.Run();