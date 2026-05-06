using Catalog.API.CatalogExtensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;

builder.Services.AddServices(builder.Configuration);
// MediateR configuration.
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});
// Fluent Validation configuration.
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Carter Library Configuration.
builder.Services.AddCarter();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();

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
app.MapCarter();
app.MapGet("/", () => "Hello World!");

app.Run();
