using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AbdusCo.CronJobs
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public JobFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public IJob Create(string jobName)
        {
            var type = Assembly.GetEntryAssembly()!
                .GetTypes()
                .FirstOrDefault(t =>
                    t.IsClass
                    && typeof(IJob).IsAssignableFrom(t)
                    && string.Equals(t.Name, jobName, StringComparison.InvariantCultureIgnoreCase));

            if (type == null)
            {
                throw new TypeLoadException($"Cannot find any job named {jobName} that implements {nameof(IJob)}");
            }

            return (IJob) _scopeFactory.CreateScope().ServiceProvider.GetRequiredService(type);
        }
    }
}