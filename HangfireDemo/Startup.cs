using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HangfireDemo.Jobs;
using HangfireDemo.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace HangfireDemo
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
            services.AddHttpContextAccessor();
            services.AddTransient<CreateReport>();
            services.AddSingleton<TriggerableJobFactory>();
            services.AddScoped<IJobProvider, ControllerActionJobProvider>();
            services.AddScoped<IJobProvider, TriggerableJobProvider>(provider =>
            {
                var request = provider.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Request;
                var uriBuilder = new UriBuilder(request.GetEncodedUrl()) {Path = "/-/jobs", Query = "", Fragment = ""};
                return new TriggerableJobProvider(uriBuilder.Uri.ToString());
            });
            services.AddTransient<ITriggerableJob, CreateReport>();

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "HangfireDemo", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "";
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HangfireDemo v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}