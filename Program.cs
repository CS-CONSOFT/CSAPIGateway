using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot();
builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API CSBS101"
    });
    c.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            return [api.GroupName];
        }

        if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            return [controllerActionDescriptor.ControllerName];
        }

        throw new InvalidOperationException("Unable to determine tag for endpoint.");
    });
    c.DocInclusionPredicate((name, api) => true);
});



if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    builder.WebHost.UseUrls(["http://0.0.0.0:80"]);
}

if (builder.Environment.IsProduction())
{
    builder.WebHost.UseUrls(["http://0.0.0.0:443"]);
}




var app = builder.Build();

app.UseSwagger(options =>
{
    options.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        swagger.Servers = new List<OpenApiServer>();
        if (app.Environment.IsDevelopment())
        {
            swagger.Servers.Add(new OpenApiServer
            {
                Url = $"{httpReq.Scheme}://{httpReq.Host.Value}",
                Description = "DEV",
            });
            swagger.Servers.Add(new OpenApiServer
            {
                Url = "http://api-uat.com",
                Description = "UAT",
            });
        }
        if (app.Environment.IsProduction())
        {
            swagger.Servers.Add(new OpenApiServer
            {
                Url = $"{httpReq.Scheme}://{httpReq.Host.Value}",
                Description = "Production",
            });
            swagger.Servers.Add(new OpenApiServer
            {
                Url = "http://api-uat.com",
                Description = "UAT",
            });
        }
    });
});


app.UseCors();
app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
});




await app.UseOcelot(); // Depois, execute o Ocelot

app.Run();
