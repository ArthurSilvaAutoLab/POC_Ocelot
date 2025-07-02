using ApiGateway.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OpenTelemetry.Resources;

using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Enrichers.Span;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => 
    {
        builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

builder.Services.AddSerilog(new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.GrafanaLoki(
        builder.Configuration["Loki:Uri"]!,
        new List<LokiLabel>()
        {
            new()
            {
                Key = "service_name",
                Value = "Gateway"
            },
            new()
            {
                Key = "using_database",
                Value = "true"
            }
        })
    .Enrich.WithSpan(new SpanOptions() { IncludeOperationName = true, IncludeTags = true })
    .CreateLogger());

var app = builder.Build();

app.UseHttpsRedirection();

app.UseMiddleware<InterceptionMiddleware>();
app.UseAuthorization();

app.UseSerilogRequestLogging();

app.UseOcelot().Wait();

app.Run();
