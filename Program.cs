using Okoora.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Register services as singletons to share data
builder.Services.AddSingleton<IRateFetcher, RateFetcher>();
builder.Services.AddSingleton<IRatePrinter, RatePrinter>();

// Register the hosted service
builder.Services.AddHostedService(serviceProvider => 
    (RateFetcher)serviceProvider.GetRequiredService<IRateFetcher>());

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.MapControllers();

// Log startup
app.Logger.LogInformation("Starting Okoora Exchange Rate API...");

try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Application failed to start");
    throw;
}