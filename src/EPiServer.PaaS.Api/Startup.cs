using EPiServer.PaaS.Api.Business;
using EPiServer.PaaS.Api.Middleware;
using EPiServer.PaaS.Api.ResourceManager;
using EPiServer.PaaS.Api.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPiServer.PaaS.Api
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

            Credential credential = new Credential();
            Configuration.Bind("Credential", credential);
            services.AddSingleton(credential);

            Subscription subscription = new Subscription();
            Configuration.Bind("Subscription", subscription);
            services.AddSingleton(subscription);

            services.AddSingleton<SecuredHttpRequestMessageFactory>();
            services.AddSingleton<DeploymentRepository>();
            services.AddSingleton<IContainerResourceProvider, AzureContainerInstance>();

            // Hard-coded customer information!!!
            CustomerInfo customerInfo = new CustomerInfo();
            Configuration.Bind("CustomerInfo", customerInfo);
            services.AddSingleton(customerInfo);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<EnsureAccessTokenMiddleware>();

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
