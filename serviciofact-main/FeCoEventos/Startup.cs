using FeCoEventos.Application.Interface;
using FeCoEventos.Application.Main;
using FeCoEventos.Domain.Core;
using FeCoEventos.Domain.Interface;
using FeCoEventos.Infrastructure.AzureStorage;
using FeCoEventos.Infrastructure.AzureStorage.Interface;
using FeCoEventos.Infrastructure.Data.Context;
using FeCoEventos.Infrastructure.SiteRemote;
using FeCoEventos.Infrastructure.SiteRemote.Interface;
using FeCoEventos.Util.TableLog;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using TFHKA.EventsDian.Infrastructure.Data.Context;
using TFHKA.Storage.Fileshare;
using TFHKA.Storage.Fileshare.Client;
using TFHKA.Storage.Fileshare.Client.Interface;
using TFHKA.Storage.Fileshare.Interface;

namespace FeCoEventos
{
    public class Startup
    {
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //Auth Bearer Jwt 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddCookie()
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateActor = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Token:Issuer"],
                        ValidAudience = Configuration["Token:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:Key"]))
                    };
                });
            //Health Check
            services.AddHealthChecks()
                 .AddSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    name: "Base de datos Factoring",
                    tags: new string[] { "Database" })

                 .AddSqlServer(Configuration.GetConnectionString("EmisionConnection"),
                    name: "Base de Datos Emision",
                    tags: new string[] { "Database" })

                 .AddAzureBlobStorage(
                $"DefaultEndpointsProtocol=https;AccountName={Configuration["StorageAzure:AccountName"]};AccountKey={Configuration["StorageAzure:AccountKey"]};EndpointSuffix=core.windows.net",
                    name: "AzureStorage Factoring",
                    tags: new string[] { "storage" })

                 .AddUrlGroup(new Uri(Configuration["url:Signed.url"]),
                    name: Configuration["url:Signed.url"],
                    tags: new string[] { "url", "firmado" })

                 .AddUrlGroup(new Uri(Configuration["url:SendEmail.url"]),
                    name: Configuration["url:SendEmail.url"],
                    tags: new string[] { "url", "entrega correo" })

                 .AddUrlGroup(new Uri(Configuration["url:CertificateUrl"]),
                    name: Configuration["url:CertificateUrl"],
                    tags: new string[] { "url", "certificado" });

            //TODO No responde 200
            /*
            .AddUrlGroup(new Uri(Configuration["url:ComunicacionesDianUrl"]),
               name: Configuration["url:ComunicacionesDianUrl"],
               tags: new string[] { "url", "comunicaciones dian" })
            */

            //TODO No responde 200
            /* .AddUrlGroup(new Uri(Configuration["url:AttachedDocument.url"]),
               name: Configuration["url:AttachedDocument.url"],
               tags: new string[] { "url", "attached document" }); */


            services.AddDbContext<ApplicationDbContext>(options =>
                  options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<EmisionDbContext>(options =>
                  options.UseSqlServer(Configuration.GetConnectionString("EmisionConnection")));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddSingleton<IConfiguration>(Configuration);

            //Application
            services.AddScoped<IEventList, EventList>();
            services.AddScoped<IEventCreate, EventCreate>();
            services.AddScoped<IEventStatus, EventStatus>();
            services.AddScoped<IEventUpdate, EventUpdate>();
            services.AddScoped<IEnableDianSummary, EnableDianSummary>();
            services.AddScoped<IDownloadFile, DownloadFile>();

            //Domain
            services.AddScoped<IEventFileDomain, EventFileDomain>();
            services.AddScoped<IEventListDomain, EventListDomain>();
            services.AddScoped<IEventCreateDomain, EventCreateDomain>();
            services.AddScoped<IDocumentBuild, DocumentBuild>();
            services.AddScoped<INotificationEmail, NotificationEmail>();
            services.AddScoped<IEventStatusDomain, EventStatusDomain>();
            services.AddScoped<IEventUpdateDomain, EventUpdateDomain>();
            services.AddScoped<IEnableSummaryDomain, EnableSummaryDomain>();
            services.AddScoped<IDownloadFileDomain, DownloadFileDomain>();
            services.AddScoped<IFilesDomain, FilesDomain>();

            //Infraestucture            
            services.AddScoped<ILogAzure, LogAzure>();
            services.AddScoped<IFileShareClass, FileShareClass>();
            services.AddScoped<IManageFile, ManageFile>();
            services.AddScoped<IStorageFiles, StorageFiles>();
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IEmisionDbContext, EmisionDbContext>();

            //Infraestucture Site Remote
            services.AddScoped<IRestClient, RestClient>();
            services.AddScoped<IApiRestClient, ApiRestClient>();
            services.AddScoped<ISignedClient, SignedClient>();
            services.AddScoped<IEmailClient, EmailClient>();
            services.AddScoped<ICertificatesClient, CertificatesClient>();
            services.AddScoped<IAttachedDocumentClient, AttachedDocumentClient>();
            services.AddScoped<IValidationXMLClient, ValidationXMLClient>();
            services.AddScoped<IEventXmlClient, EventXmlClient>();

            services.AddControllersWithViews();
            services.AddSwaggerGen(
            c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = $"v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}",
                    Title = "API Eventos - " + _env.EnvironmentName,
                    Description = "Permite la gestión de eventos DIAN y RADIAN para emision y recepcion",
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.EnableAnnotations();
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthorization();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Eventos");
                c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
                c.RoutePrefix = string.Empty;
                c.DocumentTitle = "API Eventos";
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            // HealthCheck middleware
            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }
    }
}
