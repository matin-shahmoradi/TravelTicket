using BuildingBlocks.Infrastracture.CorrelationId;
using BuildingBlocks.Infrastracture.Outbox.Extensions;
using Catalog.API.CatalogExtensions;
using Catalog.API.Grpc;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;
string serviceName = "TravelTicket.Catalog";

builder.Services.AddServices(builder.Configuration, builder.Environment);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
    config.WriteTo.OpenTelemetry(
        endpoint: "http://127.0.0.1:18889",
        protocol: OtlpProtocol.Grpc);
});
builder.Logging.AddOpenTelemetry(options =>
{
    options
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddOtlpExporter();
});

// Add services to the container.

var app = builder.Build();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    await app.Populate();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Configure the HTTP request pipeline
if (!app.Environment.IsEnvironment("test"))
{
    app.UseHealthChecks("/health-catalog", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
}

app.UseBackgroundJobs("catalog-outbox-processor");
app.UseCorrelationId();
app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();
app.MapHangfireDashboard(options: new DashboardOptions
{
    Authorization = []
});
app.MapGrpcService<CatalogRpcService>();
app.MapGet("/", () => "Hello World!");

app.Run();

public partial class Program
{

}
