using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot();
builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Isso é importante para o Swagger funcionar



builder.WebHost.UseUrls(["http://0.0.0.0:80"]);

var app = builder.Build();

app.UseSwagger(); // Primeiro, habilite o Swagger


//app.Map("/swagger/v1/swagger.json", b =>
//{
//    b.Run(async x =>
//    {
//        var json = File.ReadAllText("swagger.json");
//        await x.Response.WriteAsync(json);
//    });
//});

app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
});


await app.UseOcelot(); // Depois, execute o Ocelot

app.Run();
