using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);




// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Configuration.AddJsonFile("configuration.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot();


var app = builder.Build();

// Configure Swagger first
app.UseSwagger();
app.UseSwaggerUI();

// Then configure Ocelot
await app.UseOcelot();

// Configure other middlewares
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

