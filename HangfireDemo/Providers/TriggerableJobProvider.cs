using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using HangfireDemo.Jobs;

namespace HangfireDemo.Providers
{
    public class TriggerableJobProvider : IJobProvider
    {
        private readonly Uri _baseUrl;

        public TriggerableJobProvider(string baseUrl)
        {
            _baseUrl = new Uri(baseUrl);
        }

        public IEnumerable<JobDescription> Jobs => GetJobs();

        private IEnumerable<JobDescription> GetJobs()
        {
            return Assembly.GetEntryAssembly()!.GetTypes()
                .Where(t => t.IsClass
                            && typeof(ITriggerableJob).IsAssignableFrom(t)
                            && CustomAttributeExtensions.GetCustomAttribute<CronAttribute>((MemberInfo) t) != null)
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

            var uriBuilder = new UriBuilder(_baseUrl);
            uriBuilder.Path += $"/{type.Name}";
            var url = uriBuilder.Uri.AbsoluteUri;

            return new(url, type.Name, description, cron.CronExpressions);
        }
    }
}