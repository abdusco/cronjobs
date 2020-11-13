using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace AbdusCo.CronJobs.AspNetCore
{
    public class JobRegistrationBroadcaster : IJobBroadcaster
    {
        private readonly HttpClient _httpClient;
        private readonly IHostEnvironment _environment;

        public JobRegistrationBroadcaster(HttpClient httpClient,
            IHostEnvironment environment)
        {
            _httpClient = httpClient;
            _environment = environment;
        }

        public Task BroadcastAsync(IEnumerable<JobDescription> jobs, CancellationToken cancellationToken)
        {
            var payload = new JobBroadcast(
                _environment.ApplicationName,
                _environment.EnvironmentName,
                jobs
            );

            return _httpClient.PostAsJsonAsync("", payload, cancellationToken: cancellationToken);
        }
    }
}