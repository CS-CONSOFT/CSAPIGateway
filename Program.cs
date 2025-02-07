using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);




// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true);

builder.Services.AddHttpClient("OcelotHttpClient")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });



builder.Services.AddOcelot();

builder.WebHost.UseUrls(["http://0.0.0.0:80"]);

var app = builder.Build();

// Configure Swagger first
app.UseSwagger();
app.UseSwaggerUI();



// Configure other middlewares
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
// Then configure Ocelot
await app.UseOcelot();
app.Run();

