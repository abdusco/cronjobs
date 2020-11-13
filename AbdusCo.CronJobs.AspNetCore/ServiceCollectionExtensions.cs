using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace AbdusCo.CronJobs.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCronJobs(
            this IServiceCollection services,
            Action<JobsOptions> configure = null,
            params Assembly[] assemblies
        )
        {
            if (configure != null)
            {
                services.Configure(configure);
            }


            services.AddHttpClient<IJobBroadcaster, JobRegistrationBroadcaster>((provider, client) =>
                {
                    var options = provider.GetRequiredService<IOptions<JobsOptions>>().Value;
                    client.BaseAddress = new Uri(options.RegistrationEndpoint);
                    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
                })
                .AddPolicyHandler((provider, _) =>
                {
                    var options = provider.GetRequiredService<IOptions<JobsOptions>>().Value;
                    var builder = HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .WaitAndRetryAsync(options.RetryCount, i => TimeSpan.FromSeconds(Math.Pow(2, i)));
                    return builder;
                });

            services.AddHostedService<JobBroadcasterService>();
            services.AddSingleton<IJobFactory, JobFactory>();
            if (assemblies.Any())
            {
                services.AddTransient<IJobProvider, AssemblyScanningJobProvider>(delegate(IServiceProvider provider)
                {
                    var options = provider.GetRequiredService<IOptions<JobsOptions>>();
                    return new AssemblyScanningJobProvider(options, assemblies);
                });
            }
            else
            {
                services.AddTransient<IJobProvider, AssemblyScanningJobProvider>();
            }

            return services;
        }

        public static IServiceCollection AddCronJobs(this IServiceCollection services)
            => AddCronJobs(services, assemblies: Assembly.GetEntryAssembly());
    }
}