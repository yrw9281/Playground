using Dapper;

using Microsoft.Data.SqlClient;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

var otlpEndpoint = builder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!;
var serviceName = builder.Configuration.GetValue("ServiceName", defaultValue: "db-proxy-service")!;
var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";
var serviceInstanceId = Environment.MachineName;

Action<ResourceBuilder> configureResource = r => r.AddService(
    serviceName: serviceName,
    serviceVersion: serviceVersion,
    serviceInstanceId: Environment.MachineName);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
    .WriteTo.File("logs/log-.json", rollingInterval: RollingInterval.Day)
    .WriteTo.OpenTelemetry(options =>
    {
        options.Endpoint = otlpEndpoint;
        options.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = serviceName,
            ["service.version"] = serviceVersion,
            ["service.instance.id"] = Environment.MachineName
        };
    })
    .CreateLogger();
builder.Host.UseSerilog();

// OpenTelemetry Tracing
builder.Services.AddOpenTelemetry()
    .ConfigureResource(configureResource)
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSqlClientInstrumentation(opts =>
            {
                opts.SetDbStatementForText = true;
            })
            .AddOtlpExporter(opts =>
            {
                opts.Endpoint = new Uri(otlpEndpoint);
            });
    })
    .WithMetrics(meterProviderBuilder =>
    {
        meterProviderBuilder
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(opts =>
            {
                opts.Endpoint = new Uri(otlpEndpoint);
            });
    });

var app = builder.Build();

app.MapGet("/getAreas", async (ILogger<Program> logger) =>
{
    var connectionString = "Server=localhost;Database=db_smx_poc_2;User Id=sa;Password=Passw0rd!;Encrypt=False;";

    using var connection = new SqlConnection(connectionString);
    var areas = await connection.QueryAsync<Area>(
        @"SELECT Id, Section1, Section2, Section3 FROM dbo.Area"
    );

    logger.LogWarning("Retrieved {Count} areas from the database, All: {@areas}", areas.Count(), areas);

    return Results.Ok(areas);
});

app.Run();

public class Area
{
    public Guid Id { get; set; }
    public string Section1 { get; set; } = default!;
    public string Section2 { get; set; } = default!;
    public string Section3 { get; set; } = default!;
}