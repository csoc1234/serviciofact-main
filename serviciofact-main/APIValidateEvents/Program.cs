using APIValidateEvents.Application.Interface;
using APIValidateEvents.Application.Main;
using APIValidateEvents.Application.Mapping;
using APIValidateEvents.Domain.Core;
using APIValidateEvents.Domain.Interface;
using APIValidateEvents.Infrastucture.SiteRemote;
using APIValidateEvents.Infrastucture.SiteRemote.Interface;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//Health Check
builder.Services.AddHealthChecks().
        AddUrlGroup(new Uri(builder.Configuration["Endpoint:StatusDianUrl"]),
        name: "APIFacturas",
        tags: new string[] { "url" });

// Add services to the container.
builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = $"v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}",
            Title = "API Validate Events - " + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            Description = "API to Validate Events.",
        });
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    });

builder.Services.AddAutoMapper(typeof(MappingProfile));

//Application
builder.Services.AddScoped<IInvoiceCheck, InvoiceCheck>();

//Infraestructure
builder.Services.AddScoped<IBaseHttpClient, BaseHttpClient>();
builder.Services.AddScoped<IDianStatusClient, DianStatusClient>();
builder.Services.AddScoped<IValidationEvents, ValidationEvents>();

builder.Services.AddHttpClient();

WebApplication app = builder.Build();

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
    options.DocumentTitle = "API Validate Events";
});

//HealthCheck middleware
app.UseHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseAuthorization();

app.MapControllers();

app.Run();
