using Basket.API.BasketExtensions;

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapCarter();
app.MapGet("/", () => "Basket.API");

app.Run();
