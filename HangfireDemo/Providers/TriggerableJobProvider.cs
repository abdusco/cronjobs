using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using HangfireDemo.Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HangfireDemo.Providers
{
    public class TriggerableJobProvider : IJobProvider
    {
        private readonly LinkGenerator _linkGenerator;

        public TriggerableJobProvider(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public IEnumerable<JobDescription> Jobs => GetJobs();

        private IEnumerable<JobDescription> GetJobs()
        {
            return Assembly.GetEntryAssembly()!.GetTypes()
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

            var url = _linkGenerator.GetPathByAction(
                new DefaultHttpContext(),
                nameof(JobsController.ExecuteJob),
                nameof(JobsController).Replace("Controller", ""),
                new {jobName = type.Name}
            );

            return new(url, type.Name, description, cron.CronExpressions);
        }
    }
}