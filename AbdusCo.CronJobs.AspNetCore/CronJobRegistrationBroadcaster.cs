using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace AbdusCo.CronJobs.AspNetCore
{
    internal class CronJobRegistrationBroadcaster : ICronJobBroadcaster
    {
        private readonly HttpClient _httpClient;
        private readonly IHostEnvironment _environment;

        public CronJobRegistrationBroadcaster(HttpClient httpClient,
            IHostEnvironment environment)
        {
            _httpClient = httpClient;
            _environment = environment;
        }

        public Task BroadcastAsync(IEnumerable<CronJobDescription> jobs, CancellationToken cancellationToken)
        {
            var payload = new CronJobBroadcast
            {
                Application = _environment.ApplicationName,
                Environment = _environment.EnvironmentName,
                Jobs = jobs
            };

            return _httpClient.PostAsJsonAsync("", payload, cancellationToken: cancellationToken);
        }
    }
}