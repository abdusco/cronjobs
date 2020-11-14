using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AbdusCo.CronJobs.AspNetCore
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapCronJobWebhook(this IEndpointRouteBuilder endpoints,
            string endpoint = "/-/jobs")
        {
            endpoints.MapGet(endpoint, context =>
            {
                var providers = context.RequestServices.GetRequiredService<IEnumerable<ICronJobProvider>>();
                var jobs = providers.SelectMany(p => p.CronJobs).ToList();
                return context.Response.WriteAsJsonAsync(jobs);
            });

            return endpoints.MapPost($"{endpoint}/{{name:required}}", context =>
            {
                if (!(context.GetRouteValue("name") is string jobName))
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return Task.CompletedTask;
                }

                var factory = context.RequestServices.GetRequiredService<ICronJobFactory>();
                var executor = context.RequestServices.GetRequiredService<ICronJobExecutor>();

                context.Response.OnCompleted(() =>
                {
                    var job = factory.Create(jobName);
                    return executor.ExecuteJobAsync(job, context.RequestAborted);
                });

                context.Response.StatusCode = StatusCodes.Status202Accepted;
                return Task.CompletedTask;
            });
        }
    }
}