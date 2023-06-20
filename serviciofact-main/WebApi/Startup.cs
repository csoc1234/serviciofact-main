using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using TFHKA.LogsMongo;
using TFHKA.LogsMongo.Domain.Core;
using TFHKA.LogsMongo.Domain.Interface;
using TFHKA.LogsMongo.Infraestructure.Interface;
using TFHKA.LogsMongo.Infraestructure.SiteRemote;
using TFHKA.Storage.Fileshare;
using TFHKA.Storage.Fileshare.Client;
using TFHKA.Storage.Fileshare.Client.Interface;
using TFHKA.Storage.Fileshare.Interface;
using WebApi.Application;
using WebApi.Application.Interface;
using WebApi.Application.Invoice;
using WebApi.Domain.Core;
using WebApi.Domain.Interface;
using WebApi.Infrastructure.AzureStorage;
using WebApi.Infrastructure.AzureStorage.Interface;
using WebApi.Infrastructure.ComunicationDian;
using WebApi.Infrastructure.Data.Context;

namespace WebApi
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
            services.AddSingleton<IConfiguration>(Configuration);

            //Application
            services.AddScoped<IGestionInvoice, GestionInvoice>();
            services.AddScoped<IInvoiceEventsStatus, InvoiceEventsStatus>();

            //Domain
            services.AddScoped<IInvoiceDomain, Invoice21Domain>();
            services.AddScoped<IInvoiceEventsStatusDomain, InvoiceEventsStatusDomain>();
            services.AddScoped<ICheckStatusEventsDomain, CheckStatusEventsDomain>();
            services.AddScoped<IInvoiceLastStatusDomain, InvoiceLastStatusDomain>();

            //Infraestructure
            services.AddScoped<IStorageFiles, StorageFiles>();
            services.AddScoped<IFileShareClass, FileShareClass>();
            services.AddScoped<IManageFile, ManageFile>();

            services.AddScoped<IDianStatusRestClient, DianStatusRestClient>();
            services.AddScoped<IFactoringDbContext, FactoringDbContext>();

            services.AddScoped<ILogMongo, LogMongo>();
            services.AddScoped<ISetupLog, SetupLog>();
            services.AddScoped<IApiLogs, ApiLogs>();
            services.AddScoped<IBaseHttpClient, BaseHttpClient>();

            services.AddHttpClient();

            services.AddRazorPages();

            services.AddControllers();

            //Health Check
            services.AddHealthChecks()
                //Base de Datos
                .AddSqlServer(Configuration.GetConnectionString("EmisionConnection"),
                    name: "Base de Datos Emision",
                    tags: new string[] { "Database" })
                //Base de Datos
                .AddSqlServer(Configuration.GetConnectionString("FactoringConnection"),
                    name: "Base de Datos Factoring",
                    tags: new string[] { "Database" })
                //Azure Storage
                .AddAzureBlobStorage(
                    $"DefaultEndpointsProtocol=https;AccountName={Configuration["StorageFactoring:AccountName"]};AccountKey={Configuration["StorageFactoring:AccountKey"]};EndpointSuffix=core.windows.net",
                    name: "Azure Storage Factoring",
                    tags: new string[] { "storage" })
                //Azure Storage
                .AddAzureBlobStorage(
                    $"DefaultEndpointsProtocol=https;AccountName={Configuration["StorageEmision:AccountName"]};AccountKey={Configuration["StorageEmision:AccountKey"]};EndpointSuffix=core.windows.net",
                    name: "Azure Storage Emision",
                    tags: new string[] { "storage" })
                //Azure Storage
                .AddAzureBlobStorage(
                    $"DefaultEndpointsProtocol=https;AccountName={Configuration["StorageRecepcion:AccountName"]};AccountKey={Configuration["StorageRecepcion:AccountKey"]};EndpointSuffix=core.windows.net",
                    name: "Azure Storage Recepcion",
                    tags: new string[] { "storage" });

            services.AddDbContext<EmisionDbContext>(options =>
                  options.UseSqlServer(Configuration.GetConnectionString("EmisionConnection")));

            services.AddDbContext<FactoringDbContext>(options =>
                  options.UseSqlServer(Configuration.GetConnectionString("FactoringConnection")));

            services.AddSwaggerGen(
            c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = $"v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}",
                    Title = "API de Facturas - " + _env.EnvironmentName,
                    Description = "Informa sobre las Facturas Negociables para RADIAN",
                });

                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.EnableAnnotations();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Facturas Factoring");
                c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
                c.RoutePrefix = string.Empty;
                c.DocumentTitle = "API de Facturas";
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                endpoints.MapControllers();
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
