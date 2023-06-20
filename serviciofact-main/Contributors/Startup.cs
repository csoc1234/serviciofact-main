using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Contributors.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Contributors.Application.Interface;
using Contributors.Domain.Interface;
using Contributors.Infraestructure.Data.Context.Interface;
using Contributors.Application;
using Contributors.Domain;
using Contributors.Infraestructure.Logging.Interface;
using Contributors.Infraestructure.Logging;

namespace Contributors
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllers();

            services.AddSingleton<IConfiguration>(Configuration);

            //Application
            services.AddScoped<IEnableDianCreate, EnableDianCreate>();
            services.AddScoped<ITaxpayerListStatus, TaxpayerListStatus>();

            //Domain
            services.AddScoped<IStartEnableDianDomain, StartEnableDianDomain>();
            services.AddScoped<ITaxpayersListDomain, TaxpayersListDomain>();

            //Infraestructure
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<ILogAzure, LogAzure>();


            //Health Check
            services.AddHealthChecks()
                    //Base de Datos
                    .AddSqlServer(Configuration.GetConnectionString("FactoringConnection"),
                    name: "Base de Datos Factoring",
                    tags: new string[] { "Database" });

            services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("FactoringConnection")));

            services.AddSwaggerGen(
                  c =>
                  {
                      c.SwaggerDoc("v1", new OpenApiInfo
                      {
                          Version = $"v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}",
                          Title = "API de Contribuyentes Factoring",
                          Description = "Informa sobre los Contribuyentes Emisores de la alinza para Factoring",
                      });

                      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
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

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Contribuyentes Factoring");
                c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
                c.RoutePrefix = string.Empty;
                c.DocumentTitle = "API de Contribuyentes Factoring";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
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
