using System;
using System.Linq;
using System.Reflection;
using HangfireDemo.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace HangfireDemo
{
    public class TriggerableJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public TriggerableJobFactory(IServiceProvider serviceProvider)
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
                throw new NotImplementedException($"{type} does not implement {nameof(IJob)}");
            }

            return (IJob) _serviceProvider.GetRequiredService(type);
        }
    }
}