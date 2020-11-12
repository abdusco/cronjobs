using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;

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

            services.AddHttpClient<IJobBroadcaster, JobRegistrationBroadcaster>(client =>
                    client.Timeout = TimeSpan.FromSeconds(3))
                .AddTransientHttpErrorPolicy(builder => builder.RetryAsync(3));

            services.AddHostedService<JobBroadcasterService>();
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddTransient<IJobProvider, TriggerableJobProvider>(provider
                => new TriggerableJobProvider(provider.GetRequiredService<IOptions<JobsOptions>>().Value.UrlTemplate, assemblies));
            return services;
        }

        public static IServiceCollection AddCronJobs(this IServiceCollection services)
            => AddCronJobs(services, assemblies: Assembly.GetEntryAssembly());
    }
}