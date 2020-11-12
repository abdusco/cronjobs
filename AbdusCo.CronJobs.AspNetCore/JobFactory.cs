using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AbdusCo.CronJobs.AspNetCore
{
    public interface IJobFactory
    {
        IJob Create(string jobName);
    }

    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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

            return (IJob) _serviceProvider.GetRequiredService(type);
        }
    }
}