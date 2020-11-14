using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AbdusCo.CronJobs.Hangfire
{
    public static class EndpointBuilderExtensions
    {
        public static IEndpointConventionBuilder MapCronJobRegistry(this IEndpointRouteBuilder endpoints, string endpoint = "/api/cronjobs")
        {
            return endpoints.MapPost(endpoint, async context =>
            {
                var payload = await JsonSerializer.DeserializeAsync<CronJobBroadcast>(context.Request.Body);
                var registry = context.RequestServices.GetRequiredService<ICronJobRegistry>();
                registry.Register(payload);
                
                context.Response.StatusCode = StatusCodes.Status204NoContent;
            });
        }
    }
}