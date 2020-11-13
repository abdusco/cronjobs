using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AbdusCo.CronJobs.AspNetCore
{
    public class JobBroadcasterService : BackgroundService
    {
        private readonly JobsOptions _options;
        private readonly IEnumerable<IJobProvider> _jobProviders;
        private readonly IJobBroadcaster _broadcaster;
        private readonly ILogger<JobBroadcasterService> _logger;

        public JobBroadcasterService(
            IOptions<JobsOptions> options,
            IEnumerable<IJobProvider> jobProviders,
            IJobBroadcaster broadcaster, ILogger<JobBroadcasterService> logger)
        {
            _options = options.Value;
            _jobProviders = jobProviders;
            _broadcaster = broadcaster;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(_options.WaitSeconds), stoppingToken);
            
            _logger.LogInformation("Finding jobs");
            List<JobDescription> jobs;
            try
            {
                jobs = _jobProviders.SelectMany(p => p.Jobs).ToList();
                _logger.LogInformation($"Found {jobs.Count} jobs. Broadcasting all jobs");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error happened while getting a list of jobs");
                throw;
            }

            try
            {
                await _broadcaster.BroadcastAsync(jobs, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Cannot broadcast jobs.");
            }
        }
    }
}