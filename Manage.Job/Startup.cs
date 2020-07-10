using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Manage.Job.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Manage.Job
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
            // TO AVOID CORS:
            // Access to XMLHttpRequest at 'https://localhost:44308/api/movies' from origin 'http://localhost:44301' has been blocked by CORS policy: 
            // Response to preflight request doesn't pass access control check: No 'Access - Control - Allow - Origin' header is present on the requested resource.

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeaders",
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
            });

            Helper.SiteUrlSpOnline = this.Configuration.GetSection("AppSettings")["SiteUrlSpOnline"];
            Helper.ImpersonateUserName = this.Configuration.GetSection("AppSettings")["ImpersonateUserName"];
            Helper.ImpersonatePwd = this.Configuration.GetSection("AppSettings")["ImpersonatePwd"];
            Helper.ListaReferenteGuid = this.Configuration.GetSection("AppSettings")["ListaReferenteGuid"];
            Helper.ListaJobSchedulerGuid = this.Configuration.GetSection("AppSettings")["ListaJobSchedulerGuid"];
            Helper.ListaJobManagerGuid = this.Configuration.GetSection("AppSettings")["ListaJobManagerGuid"];
            Helper.MailTest = this.Configuration.GetSection("AppSettings")["MailTest"];
            Helper.MailPort = this.Configuration.GetSection("AppSettings")["MailPort"];
            Helper.MailHost = this.Configuration.GetSection("AppSettings")["MailHost"];
            Helper.MailFrom = this.Configuration.GetSection("AppSettings")["MailFrom"];

            //services.Configure<JSonAsClassSettings>(Configuration.GetSection("appSettings"));
            services.AddTransient(ec => new EncryptionService(new KeyInfo("45BLO2yoJkvBwz99kBEMlNkxvL40vUSGaqr/WBu3+Vg=", "Ou3fn+I9SVicGWMLkFEgZQ==")));

            Helper.InfoKey = "45BLO2yoJkvBwz99kBEMlNkxvL40vUSGaqr/WBu3+Vg=";
            Helper.InfoIv = "Ou3fn+I9SVicGWMLkFEgZQ==";
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // InvalidOperationException: Endpoint Manage.Job.Controllers.SPReferenteController.Get (Manage.Job) contains CORS metadata, 
            // but a middleware was not found that supports CORS.
            // Configure your application startup by adding app.UseCors() inside the call to Configure(..) in the application startup code.
            // The call to app.UseAuthorization() must appear between app.UseRouting() and app.UseEndpoints(...).
            // Microsoft.AspNetCore.Routing.EndpointMiddleware.ThrowMissingCorsMiddlewareException(Endpoint endpoint)

            // TO AVOID CORS:
            // Access to XMLHttpRequest at 'https://localhost:44308/api/movies' from origin 'http://localhost:44301' has been blocked by CORS policy: 
            // Response to preflight request doesn't pass access control check: No 'Access - Control - Allow - Origin' header is present on the requested resource.

            // Shows UseCors with named policy.
            app.UseCors("AllowAllHeaders");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
