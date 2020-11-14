using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AbdusCo.CronJobs;
using Microsoft.Extensions.Logging;

namespace HangfireDemo.Jobs
{
    [Cron("*/2 * * * *")]
    [Description("Performs a task that takes really long")]
    public class ReallyLongCronJob: ICronJob
    {
        private readonly ILogger<ReallyLongCronJob> _logger;

        public ReallyLongCronJob(ILogger<ReallyLongCronJob> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var watch = Stopwatch.StartNew();
            _logger.LogInformation("starting...");
            while (watch.Elapsed < TimeSpan.FromSeconds(60) || !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                _logger.LogInformation($"still working (took {watch.Elapsed.Seconds} seconds so far)...");
            }
            _logger.LogInformation("done");
        }
    }
}