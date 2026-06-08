using BuildingBlocks.Infrastracture.CorrelationId;
using Catalog.API.CatalogExtensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;

builder.Services.AddServices(builder.Configuration);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container.

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    await app.Populate();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Configure the HTTP request pipeline
app.UseCorrelationId();
app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();
app.MapGet("/", () => "Hello World!");

app.Run();
