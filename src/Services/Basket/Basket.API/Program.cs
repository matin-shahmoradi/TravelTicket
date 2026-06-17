using Basket.API.BasketExtensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddServices(builder.Configuration);
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
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapCarter();
app.MapGet("/", () => "Basket.API");

app.Run();
