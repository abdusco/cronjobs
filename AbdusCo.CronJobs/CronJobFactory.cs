using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AbdusCo.CronJobs
{
    public class CronJobFactory : ICronJobFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CronJobFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public ICronJob Create(string jobName)
        {
            var type = Assembly.GetEntryAssembly()!
                .GetTypes()
                .FirstOrDefault(t =>
                    t.IsClass
                    && typeof(ICronJob).IsAssignableFrom(t)
                    && string.Equals(t.Name, jobName, StringComparison.InvariantCultureIgnoreCase));

            if (type == null)
            {
                throw new TypeLoadException($"Cannot find any job named {jobName} that implements {nameof(ICronJob)}");
            }

            return (ICronJob) _scopeFactory.CreateScope().ServiceProvider.GetRequiredService(type);
        }
    }
}