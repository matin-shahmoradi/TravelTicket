using Catalog.API.CatalogExtensions;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;

builder.Services.AddServices(builder.Configuration);
// MediateR configuration.
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(
        typeof(Program).Assembly,
        typeof(ValidationBehavior<,>).Assembly
    );
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
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
