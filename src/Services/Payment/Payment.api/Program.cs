using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Payment.api;
using Payment.api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.PaymentServices(builder.Configuration);
builder.Logging.AddOpenTelemetry(options =>
{
    options
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("TravelTicket.Payment"))
        .AddOtlpExporter();
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.MigrateDatabase();
}
app.UsePayments();

app.Run();
