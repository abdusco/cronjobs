using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using HangfireDemo.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HangfireDemo
{
    public interface IJobBroadcaster
    {
        Task BroadcastAsync(params JobDescription[] jobs);
    }

    public sealed record JobBroadcast(string Application, string Environment, IEnumerable<JobDescription> Jobs);

    public class HangfireJobBroadcaster : IJobBroadcaster
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HangfireConfig _config;
        private readonly IHostEnvironment _environment;

        public HangfireJobBroadcaster(IOptions<HangfireConfig> config, IHttpClientFactory httpClientFactory,
            IHostEnvironment environment)
        {
            _config = config.Value;
            _httpClientFactory = httpClientFactory;
            _environment = environment;
        }

        public async Task BroadcastAsync(params JobDescription[] jobs)
        {
            using var http = _httpClientFactory.CreateClient();
            http.Timeout = TimeSpan.FromSeconds(_config.Timeout);

            var payload = new JobBroadcast(
                _environment.ApplicationName,
                _environment.EnvironmentName,
                jobs
            );

            var res = await http.PostAsJsonAsync(_config.RegistrationEndpoint, payload);
            res.EnsureSuccessStatusCode();
        }
    }

    public class JobBroadcasterService : BackgroundService
    {
        private readonly IEnumerable<IJobProvider> _jobProviders;
        private readonly IJobBroadcaster _broadcaster;
        private readonly ILogger<JobBroadcasterService> _logger;

        public JobBroadcasterService(
            IEnumerable<IJobProvider> jobProviders,
            IJobBroadcaster broadcaster, ILogger<JobBroadcasterService> logger)
        {
            _jobProviders = jobProviders;
            _broadcaster = broadcaster;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Finding jobs");
            var jobs = _jobProviders.SelectMany(p => p.Jobs).ToArray();
            _logger.LogInformation($"Found {jobs.Length} jobs. Broadcasting all jobs");
            try
            {
                await _broadcaster.BroadcastAsync(jobs);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Cannot broadcast jobs. Got HTTP {e.StatusCode} response");
            }
        }
    }
}