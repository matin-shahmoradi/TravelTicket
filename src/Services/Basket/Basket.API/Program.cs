using Basket.API.BasketExtensions;
using BuildingBlocks.Infrastracture.CorrelationId;
using BuildingBlocks.Infrastracture.Outbox.Extensions;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddServices(builder.Configuration, builder.Environment);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MigrateDatabase().GetAwaiter().GetResult();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHealthChecks("/health-basket", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseExceptionHandler();
app.UseBackgroundJobs("basket-outbox-processor");
app.UseCorrelationId();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();
app.MapHangfireDashboard(options: new DashboardOptions
{
    Authorization = []
});
app.MapGet("/", () => "Basket.API");

app.Run();
