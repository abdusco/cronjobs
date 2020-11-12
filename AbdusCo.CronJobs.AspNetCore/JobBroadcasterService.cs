using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AbdusCo.CronJobs.AspNetCore
{
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
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            
            _logger.LogInformation("Finding jobs");
            var jobs = _jobProviders.SelectMany(p => p.Jobs).ToList();
            _logger.LogInformation($"Found {jobs.Count} jobs. Broadcasting all jobs");
            try
            {
                await _broadcaster.BroadcastAsync(jobs);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Cannot broadcast jobs.");
            }
        }
    }
}