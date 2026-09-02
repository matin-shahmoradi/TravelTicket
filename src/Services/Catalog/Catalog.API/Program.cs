using BuildingBlocks.Infrastracture.CorrelationId;
using BuildingBlocks.Infrastracture.Outbox.Extensions;
using Catalog.API.CatalogExtensions;
using Catalog.API.Grpc;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;

builder.Services.AddServices(builder.Configuration, builder.Environment);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
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
