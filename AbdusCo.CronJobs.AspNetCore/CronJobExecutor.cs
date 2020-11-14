using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AbdusCo.CronJobs.AspNetCore
{
    public class CronJobExecutor: ICronJobExecutor
    {
        private readonly ILogger<CronJobExecutor> _logger;

        public CronJobExecutor(ILogger<CronJobExecutor> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteJobAsync(ICronJob cronJob, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Executing {cronJob}");
            await cronJob.ExecuteAsync(cancellationToken);
            _logger.LogInformation($"Finished {cronJob}");
        }
    }
}