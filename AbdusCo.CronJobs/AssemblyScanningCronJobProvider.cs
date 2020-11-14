using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace AbdusCo.CronJobs
{
    public class AssemblyScanningCronJobProvider : ICronJobProvider
    {
        private readonly string _urlTemplate;
        private readonly Assembly[] _assemblies;

        public AssemblyScanningCronJobProvider(IOptions<CronJobsOptions> options, params Assembly[] assemblies)
        {
            _urlTemplate = options.Value.UrlTemplate;
            _assemblies = assemblies;
        }

        public IEnumerable<CronJobDescription> CronJobs => GetJobs();

        private IEnumerable<CronJobDescription> GetJobs()
        {
            return _assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass
                            && !t.IsAbstract
                            && t.IsPublic
                            && typeof(ICronJob).IsAssignableFrom(t))
                .Select(CreateJobDescription);
        }

        private CronJobDescription CreateJobDescription(Type type)
        {
            var cron = type.GetCustomAttribute<CronAttribute>();
            if (cron == null)
            {
                throw new TypeAccessException($"{type} does not have any {nameof(CronAttribute)} attribute");
            }

            var description = type.GetCustomAttribute<DescriptionAttribute>()?.Description
                              ?? type.FullName;

            return new CronJobDescription
            {
                Name = type.Name,
                Endpoint = _urlTemplate.Replace("{name}", type.Name.ToLowerInvariant()),
                CronExpressions = cron.CronExpressions.ToList(),
                Description = description
            };
        }
    }
}