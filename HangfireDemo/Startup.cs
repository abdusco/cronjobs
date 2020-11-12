using HangfireDemo.Jobs;
using HangfireDemo.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            services.Configure<HangfireConfig>(Configuration.GetSection(HangfireConfig.Key));
            services.AddHttpClient();
            
            // services.AddHostedService<JobBroadcasterService>();
            services.AddTransient<IJobBroadcaster, HangfireJobBroadcaster>();
            services.AddTransient<CreateReport>();
            services.AddSingleton<TriggerableJobFactory>();
            services.AddTransient<IJobBroadcaster, HangfireJobBroadcaster>();
            services.AddTransient<IJobProvider, ControllerActionJobProvider>();
            services.AddTransient<IJobProvider, TriggerableJobProvider>();
            services.AddTransient<IJob, CreateReport>();

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