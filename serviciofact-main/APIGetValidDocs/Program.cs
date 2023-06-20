using APIGetValidDocs.Application.Interface;
using APIGetValidDocs.Application.Main;
using APIGetValidDocs.Application.Mapping;
using APIGetValidDocs.Domain.Core;
using APIGetValidDocs.Domain.Interface;
using APIGetValidDocs.Infraestructure.AzureStorage;
using APIGetValidDocs.Infraestructure.Database;
using APIGetValidDocs.Infraestructure.Interface;
using APIGetValidDocs.Infraestructure.SiteRemote;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TFHKA.Storage.Fileshare;
using TFHKA.Storage.Fileshare.Client;
using TFHKA.Storage.Fileshare.Client.Interface;
using TFHKA.Storage.Fileshare.Interface;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//Health Check
builder.Services.AddHealthChecks().
    AddUrlGroup(new Uri(builder.Configuration["Endpoint:ValidateEvents"]),
        name: "ValidateEventsApi",
        tags: new string[] { "url" })
    .AddSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"],
                    name: "BD de Factoring",
                    tags: new string[] { "Database" });

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
            Title = "API Get Validate Docs  - " + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            Description = "Validaciónn de documentos.",
        });
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    });

//Scoped
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IValidDocs, ValidDocs>();
builder.Services.AddScoped<IGetInvoiceDomain, GetInvoiceDomain>();

builder.Services.AddSingleton<IAccumulatorsMetric, AccumulatorsMetric>();
builder.Services.AddScoped<IThreadCreator, ThreadCreator>();

//Infraestructure
builder.Services.AddScoped<IInvoiceClient, InvoiceClient>();
builder.Services.AddScoped<IStorageFiles, StorageFiles>();
builder.Services.AddScoped<IFileShareClass, FileShareClass>();
builder.Services.AddScoped<IBaseHttpClient, BaseHttpClient>();
builder.Services.AddScoped<IManageFile, ManageFile>();
builder.Services.AddScoped<IValidDocsDbContext, ValidDocsDbContext>();

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
    options.DocumentTitle = "API Get Valid Docs";
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
