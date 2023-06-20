using APIAttachedDocument.Application.Interface;
using APIAttachedDocument.Application.Main;
using APIAttachedDocument.Domain.Core;
using APIAttachedDocument.Domain.Interface;
using APIAttachedDocument.Infrastructure;
using APIAttachedDocument.Infrastructure.Interface;
using APIAttachedDocument.Infrastructure.Logging;
using APIAttachedDocument.Infrastructure.Signed;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Auth Bearer Jwt 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCookie()
    .AddJwtBearer(jwtBearerOptions =>
    {
        jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateActor = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Token:Issuer"],
            ValidAudience = builder.Configuration["Token:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:Key"]))
        };
    });

//Health Check
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri(builder.Configuration["url:Signed.url"]),
        name: builder.Configuration["url:Signed.url"],
        tags: new string[] { "url", "firmado" })

    .AddUrlGroup(new Uri(builder.Configuration["url:CertificateUrl"]),
        name: builder.Configuration["url:CertificateUrl"],
        tags: new string[] { "url", "certificado" });


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = $"v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}",
            Title = "API AttachedDocument - " + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            Description = "Genera XML Attached Document UBL 2.1",
        });

        c.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme
        {
            Description = "Authorization by API key.",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Name = "Authorization"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
{{              new OpenApiSecurityScheme
        {       Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "Authorization" }},
                new List<string>()
            }});
    });

//Application
builder.Services.AddScoped<ICreateDocument, CreateDocument>();

//Domain
builder.Services.AddScoped<ICreateDocumentDomain, CreateDocumentDomain>();

//Infraestructure
builder.Services.AddScoped<ILogAzure, LogAzure>();
builder.Services.AddScoped<ICertificateClient, CertificateClient>();
builder.Services.AddScoped<ISignedClient, SignedClient>();


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger(x => x.SerializeAsV2 = true);
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

//HealthCheck middleware
app.UseHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
