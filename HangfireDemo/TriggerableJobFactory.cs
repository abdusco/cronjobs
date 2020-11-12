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

        public ITriggerableJob Create(string jobName)
        {
            var type = Assembly.GetEntryAssembly()!
                .GetTypes()
                .FirstOrDefault(t => t.IsClass && typeof(ITriggerableJob).IsAssignableFrom(t) && t.Name == jobName);
            if (type == null || !typeof(ITriggerableJob).IsAssignableFrom(type))
            {
                throw new NotImplementedException($"{type} does not implement {nameof(ITriggerableJob)}");
            }

            return (ITriggerableJob) _serviceProvider.GetRequiredService(type);
        }
    }
}