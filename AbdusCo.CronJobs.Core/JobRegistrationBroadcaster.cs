using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AbdusCo.CronJobs.Core
{
    public class JobRegistrationBroadcaster : IJobBroadcaster
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JobsOptions _options;
        private readonly IHostEnvironment _environment;

        public JobRegistrationBroadcaster(IOptions<JobsOptions> config, IHttpClientFactory httpClientFactory,
            IHostEnvironment environment)
        {
            _options = config.Value;
            _httpClientFactory = httpClientFactory;
            _environment = environment;
        }

        public async Task BroadcastAsync(params JobDescription[] jobs)
        {
            using var http = _httpClientFactory.CreateClient();
            http.Timeout = TimeSpan.FromSeconds(_options.Timeout);

            var payload = new JobBroadcast(
                _environment.ApplicationName,
                _environment.EnvironmentName,
                jobs
            );

            var res = await http.PostAsJsonAsync(_options.RegistrationEndpoint, payload);
            res.EnsureSuccessStatusCode();
        }
    }
}