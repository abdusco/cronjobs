using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AbdusCo.CronJobs.AspNetCore
{
    public class JobRegistrationBroadcaster : IJobBroadcaster
    {
        private readonly JobsOptions _options;
        private readonly HttpClient _httpClient;
        private readonly IHostEnvironment _environment;

        public JobRegistrationBroadcaster(IOptions<JobsOptions> config, HttpClient httpClient,
            IHostEnvironment environment)
        {
            _options = config.Value;
            _httpClient = httpClient;
            _environment = environment;
        }

        public async Task BroadcastAsync(IEnumerable<JobDescription> jobs)
        {
            var payload = new JobBroadcast(
                _environment.ApplicationName,
                _environment.EnvironmentName,
                jobs
            );

            var res = await _httpClient.PostAsJsonAsync(_options.RegistrationEndpoint, payload);
            res.EnsureSuccessStatusCode();
        }
    }
}