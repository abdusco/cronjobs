using System;
using System.Collections;
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

    public class HangfireJobBroadcaster : IJobBroadcaster
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _endpoint;

        public HangfireJobBroadcaster(IOptions<HangfireConfig> config, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _endpoint = config.Value.RegistrationEndpoint;
        }

        public async Task BroadcastAsync(params JobDescription[] jobs)
        {
            using var http = _httpClientFactory.CreateClient();
            var res = await http.PostAsJsonAsync(_endpoint, jobs);
            res.EnsureSuccessStatusCode();
        }
    }

    public class JobBroadcasterService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IJobBroadcaster _broadcaster;
        private readonly ILogger<JobBroadcasterService> _logger;

        public JobBroadcasterService(
            IServiceProvider serviceProvider,
            IJobBroadcaster broadcaster, ILogger<JobBroadcasterService> logger)
        {
            _serviceProvider = serviceProvider;
            _broadcaster = broadcaster;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Finding jobs");
            var jobs = GetJobs();
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

        private JobDescription[] GetJobs()
        {
            using var scope = _serviceProvider.CreateScope();
            var jobProviders = scope.ServiceProvider.GetRequiredService<IEnumerable<IJobProvider>>();
            var jobs = jobProviders.SelectMany(p => p.Jobs).ToArray();
            return jobs;
        }
    }
}