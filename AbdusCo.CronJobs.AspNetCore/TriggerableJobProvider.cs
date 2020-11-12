using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace AbdusCo.CronJobs.AspNetCore
{
    public class TriggerableJobProvider : IJobProvider
    {
        private readonly string _urlTemplate;
        private readonly Assembly[] _assemblies;

        public TriggerableJobProvider(string urlTemplate, params Assembly[] assemblies)
        {
            _urlTemplate = urlTemplate;
            _assemblies = assemblies;
        }

        public TriggerableJobProvider(string urlTemplate) : this(urlTemplate, AppDomain.CurrentDomain.GetAssemblies())
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

            return new JobDescription
            {
                Name = type.Name,
                Endpoint = _urlTemplate.Replace("{name}", type.Name.ToLowerInvariant()),
                CronExpressions = cron.CronExpressions.ToList(),
                Description = description
            };
        }
    }
}