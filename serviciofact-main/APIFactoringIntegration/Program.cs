using APIFactoringIntegration.Application.Interface;
using APIFactoringIntegration.Application.Main;
using APIFactoringIntegration.Application.Mapping;
using APIFactoringIntegration.Domain.Core;
using APIFactoringIntegration.Domain.Interface;
using APIFactoringIntegration.Infraestructure.Database;
using APIFactoringIntegration.Infraestructure.Interface;
using APIFactoringIntegration.Infraestructure.SiteRemote;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Health Check
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri(builder.Configuration["Endpoint:APIGetValidDocs"]),
        name: "APIGetValidDocs",
        tags: new string[] { "url" })
    .AddSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"],
        name: "BD de Factoring",
        tags: new string[] { "Database" });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

//Application
builder.Services.AddScoped<IDocument, Document>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

//Domain
builder.Services.AddScoped<IAuthenticateDomain, AuthenticateDomain>();
builder.Services.AddScoped<IValidDocsList, ValidDocsList>();

//Infraestructure
builder.Services.AddScoped<IBaseHttpClient, BaseHttpClient>();
builder.Services.AddScoped<IAPIGetValidDocs, APIGetValidDocs>();
builder.Services.AddScoped<ICredentialsDbContext, CredentialsDbContext>();

builder.Services.AddHttpClient();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = $"v{Assembly.GetExecutingAssembly().GetName().Version}",
            Title = "API Factoring Integration  - " + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            Description = "Integracion para factoring con The Factory HKA Colombia.",
        });
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    });

WebApplication app = builder.Build();

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
    options.DocumentTitle = "API Factoring Integration";
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
