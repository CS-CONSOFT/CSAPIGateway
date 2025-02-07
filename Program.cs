using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);



builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true);


builder.Services.AddOcelot();

//builder.WebHost.UseUrls(["http://0.0.0.0:80"]);

var app = builder.Build();

// Then configure Ocelot
await app.UseOcelot();
app.Run();

