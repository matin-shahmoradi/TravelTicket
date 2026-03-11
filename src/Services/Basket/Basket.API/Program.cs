using Basket.API.Data;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;
// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddMarten(config =>
{
    config.Connection(builder.Configuration.GetConnectionString("DefaultConnection")!);
}).UseLightweightSessions();

builder.Services.AddCarter();
builder.Services.AddScoped<IBasketRepository,BasketRepository>();
builder.Services.AddSwaggerGen(cfg =>
{
    
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
