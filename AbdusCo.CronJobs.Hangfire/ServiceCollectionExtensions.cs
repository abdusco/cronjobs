using System;
using Microsoft.Extensions.DependencyInjection;

namespace AbdusCo.CronJobs.Hangfire
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCronJobServer(this IServiceCollection services)
        {
            services.AddTransient<ICronJobRegistry, CronJobRegistry>();
            services.AddHttpClient<ICronJobTriggerer, CronJobTriggerer>(client =>
                client.Timeout = TimeSpan.FromSeconds(3));
            return services;
        }
    }
}