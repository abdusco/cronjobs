using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HangfireDemo.Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HangfireDemo
{
    [Route("/-/jobs")]
    public class JobsController : Controller
    {
        private readonly TriggerableJobFactory _jobFactory;
        private readonly IEnumerable<IJobProvider> _jobProviders;
        private readonly ILogger<JobsController> _logger;

        public JobsController(TriggerableJobFactory jobFactory, IEnumerable<IJobProvider> jobProviders,
            ILogger<JobsController> logger)
        {
            _jobFactory = jobFactory;
            _jobProviders = jobProviders;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<JobDescription>>> GetTriggerableJobs()
        {
            return Json(_jobProviders.SelectMany(p => p.Jobs).ToList());
        }

        [HttpPost("{jobName}")]
        public async Task<IActionResult> ExecuteJob(string jobName)
        {
            var job = _jobFactory.Create(jobName);
            if (job == null)
            {
                return NotFound();
            }

            _logger.LogInformation($"Executing {job}");
            await job.ExecuteAsync();
            _logger.LogInformation($"Finished {job}");

            return Ok();
        }
    }

    public sealed record JobDescription(string Url, string Name, string Description, params string[] CronExpressions);

    public interface IJobProvider
    {
        IEnumerable<JobDescription> Jobs { get; }
    }

    public class ActionJobProvider : IJobProvider
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorProvider;
        private readonly LinkGenerator _linkGenerator;
        private readonly HttpContext _httpContext;

        public ActionJobProvider(IActionDescriptorCollectionProvider actionDescriptorProvider,
            LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _actionDescriptorProvider = actionDescriptorProvider;
            _linkGenerator = linkGenerator;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public IEnumerable<JobDescription> Jobs => GetAllJobs();

        private IEnumerable<JobDescription> GetAllJobs()
        {
            return _actionDescriptorProvider.ActionDescriptors.Items
                .Where(e =>
                    e.EndpointMetadata.Any(m => m is TriggerableAttribute)
                    && e.EndpointMetadata.Any(m => m is CronAttribute)
                    && e is ControllerActionDescriptor)
                .Select(e => CreateJobDescription(e as ControllerActionDescriptor));
        }

        private JobDescription CreateJobDescription(ControllerActionDescriptor action)
        {
            var url = GetUriByAction(action);
            var cronAttribute = action.EndpointMetadata
                .FirstOrDefault(m => m is CronAttribute) as CronAttribute;
            var description = action.MethodInfo.GetCustomAttribute<DescriptionAttribute>()?.Description
                              ?? action.DisplayName;

            return new(url, action.ActionName, description, cronAttribute!.CronExpressions);
        }

        private string GetUriByAction(ControllerActionDescriptor action) =>
            _linkGenerator.GetUriByAction(
                _httpContext,
                action.RouteValues["action"],
                action.RouteValues["controller"]
            );
    }

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

            var uriBuilder = new UriBuilder(_baseUrl);
            uriBuilder.Path += $"/{type.Name}";
            var url = uriBuilder.Uri.AbsoluteUri;

            return new(url, type.Name, description, cron.CronExpressions);
        }
    }

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