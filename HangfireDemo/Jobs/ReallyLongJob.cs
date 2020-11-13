using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AbdusCo.CronJobs;
using Microsoft.Extensions.Logging;

namespace HangfireDemo.Jobs
{
    [Cron("*/1 * * * *")]
    [Description("Performs a task that takes really long")]
    public class ReallyLongJob: IJob
    {
        private readonly ILogger<ReallyLongJob> _logger;

        public ReallyLongJob(ILogger<ReallyLongJob> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var watch = Stopwatch.StartNew();
            _logger.LogInformation("starting...");
            while (watch.Elapsed < TimeSpan.FromSeconds(60))
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                _logger.LogInformation("still working...");
            }
            _logger.LogInformation("done");
        }
    }
}