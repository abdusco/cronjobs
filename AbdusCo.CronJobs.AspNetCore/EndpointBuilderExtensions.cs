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
                var providers = context.RequestServices.GetRequiredService<IEnumerable<IJobProvider>>();
                var jobs = providers.SelectMany(p => p.Jobs).ToList();
                return context.Response.WriteAsJsonAsync(jobs);
            });

            return endpoints.MapPost($"{endpoint}/{{name:required}}", context =>
            {
                if (!(context.GetRouteValue("name") is string jobName))
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return Task.CompletedTask;
                }

                var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JobExecutor");
                var factory = context.RequestServices.GetRequiredService<IJobFactory>();

                context.Response.OnCompleted(async () =>
                {
                    var job = factory.Create(jobName);
                    logger.LogInformation($"Executing {job}");
                    await job.ExecuteAsync(context.RequestAborted);
                    logger.LogInformation($"Finished {job}");
                });

                context.Response.StatusCode = StatusCodes.Status202Accepted;
                return Task.CompletedTask;
            });
        }
    }
}