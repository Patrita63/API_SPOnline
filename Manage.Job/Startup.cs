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
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
