using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;

namespace HangfireDemo.Providers
{
    public class ControllerActionJobProvider : IJobProvider
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorProvider;
        private readonly LinkGenerator _linkGenerator;
        private readonly HttpContext _httpContext;

        public ControllerActionJobProvider(IActionDescriptorCollectionProvider actionDescriptorProvider,
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
}