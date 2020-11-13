using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AbdusCo.CronJobs.AspNetCore;
using Microsoft.Extensions.Logging;

namespace HangfireDemo.Jobs
{
    [Cron("*/1 * * * *")]
    public class ReallyLongJob: IJob
    {
        private readonly ILogger<ReallyLongJob> _logger;

        public ReallyLongJob(ILogger<ReallyLongJob> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            var watch = Stopwatch.StartNew();
            _logger.LogInformation("starting...");
            while (watch.Elapsed < TimeSpan.FromSeconds(60))
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                _logger.LogInformation("still working...");
            }
            _logger.LogInformation("done");
        }
    }
}