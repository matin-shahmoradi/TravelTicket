using AuthService;
using AuthService.Data;
using BuildingBlocks.Extensions;
using Carter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AuthServices(builder.Configuration);
builder.Host.UseSharedSerilog(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.SeedDb();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapCarter();
app.Run();
