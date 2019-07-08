using System;
using System.Collections.Generic;
using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: ApiController]
namespace cats_api
{
    public class Startup
    {
        private readonly IHostingEnvironment environment;
        private readonly ILoggerFactory loggerFactory;
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment environment, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.environment = environment;
            Configuration = configuration;
            this.loggerFactory = loggerFactory;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddControllersAsServices()
                    .ConfigureApiBehaviorOptions(o =>
                    {
                        o.InvalidModelStateResponseFactory = ctx =>
                        {
                            return new ValidationProblemDetailsResult();
                        };
                    });
            services.AddOptions();
            services.AddCorrelationId();
            services.Configure<CorrelationIdOptions>(Configuration.GetSection("CorrelationIdOptions"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var opts = app.ApplicationServices.GetService<IOptionsMonitor<CorrelationIdOptions>>() ?? null;
            app.UseCorrelationId(opts.CurrentValue);
            app.UseProblemDetailErrorHandler(env.IsDevelopment(), this.loggerFactory.CreateLogger<ILogger<Startup>>());
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            if (!env.IsDevelopment()) app.UseHsts();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
