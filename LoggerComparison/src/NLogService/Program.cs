using Microsoft.AspNetCore.Mvc;

using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using NLog;
using NLog.Web;

using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var otlpEndpoint = builder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!;
var serviceName = builder.Configuration.GetValue("ServiceName", defaultValue: "nlog-service")!;
var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";
var serviceInstanceId = Environment.MachineName;

// NLog Configuration
LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// OpenTelemetry
Action<ResourceBuilder> configureResource = r => r.AddService(
    serviceName: serviceName,
    serviceVersion: serviceVersion,
    serviceInstanceId: Environment.MachineName);
// OpenTelemetry Tracing
builder.Services.AddOpenTelemetry()
    .ConfigureResource(configureResource)
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(opts =>
            {
                opts.Endpoint = new Uri(otlpEndpoint);
            });
    })
    .WithLogging(logging =>
    {
        logging.AddOtlpExporter(opts =>
        {
            opts.Endpoint = new Uri(otlpEndpoint);
        });

        logging.AddConsoleExporter();
    });

var app = builder.Build();

app.MapPost("/log", ([FromBody] LogRequest request, ILogger<Program> logger) =>
{
    var stopwatch = Stopwatch.StartNew();

    for (int i = 1; i <= request.Count; i++)
    {
        var demoObject = new DemoObject
        {
            Name = $"Demo {i}",
            Value = i,
            Timestamp = DateTime.UtcNow,
            Tags = new List<string> { "example", "test", "logging" }
        };
        logger.LogWarning("{Message} - #{i}", request.Message, i);
        logger.LogError("{Message} - #{i} - {demoObject}", request.Message, i, demoObject);
    }

    stopwatch.Stop();

    return Results.Ok(new
    {
        request.Count,
        request.Message,
        stopwatch.ElapsedMilliseconds
    });
});

app.Run();

LogManager.Shutdown();

public record LogRequest(string Message, int Count);

public class DemoObject
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public List<string> Tags { get; set; } = new List<string>();
}