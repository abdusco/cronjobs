using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace AbdusCo.CronJobs.AspNetCore
{
    internal class AssemblyScanningJobProvider : IJobProvider
    {
        private readonly string _urlTemplate;
        private readonly Assembly[] _assemblies;

        public AssemblyScanningJobProvider(IOptions<JobsOptions> options, params Assembly[] assemblies)
        {
            _urlTemplate = options.Value.UrlTemplate;
            _assemblies = assemblies;
        }

        public AssemblyScanningJobProvider(IOptions<JobsOptions> options) : this(options,
            AppDomain.CurrentDomain.GetAssemblies())
        {
        }

        public IEnumerable<JobDescription> Jobs => GetJobs();

        private IEnumerable<JobDescription> GetJobs()
        {
            return _assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass
                            && !t.IsAbstract
                            && t.IsPublic
                            && typeof(IJob).IsAssignableFrom(t))
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