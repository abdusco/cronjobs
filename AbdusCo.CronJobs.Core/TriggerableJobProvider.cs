using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace AbdusCo.CronJobs.Core
{
    public class TriggerableJobProvider : IJobProvider
    {
        private readonly Assembly[] _assemblies;

        public TriggerableJobProvider(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public TriggerableJobProvider() : this(AppDomain.CurrentDomain.GetAssemblies())
        {
        }

        public IEnumerable<JobDescription> Jobs => GetJobs();

        private IEnumerable<JobDescription> GetJobs()
        {
            return _assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass
                            && typeof(IJob).IsAssignableFrom(t)
                            && t.GetCustomAttribute<CronAttribute>() != null)
                .Select(CreateJobDescription);
        }

        private JobDescription CreateJobDescription(Type type)
        {
            var cron = type.GetCustomAttribute<CronAttribute>();
            if (cron == null)
            {
                throw new TypeAccessException($"{type} does not have any {nameof(CronAttribute)} attribute");
            }

            var description = type.GetCustomAttribute<DescriptionAttribute>()?.Description
                              ?? type.FullName;

            return new JobDescription(type.Name, cron.CronExpressions)
            {
                Description = description
            };
        }
    }
}