using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AbdusCo.CronJobs.AspNetCore
{
    public class CronCronJobExecutor: ICronJobExecutor
    {
        private readonly ILogger<CronCronJobExecutor> _logger;

        public CronCronJobExecutor(ILogger<CronCronJobExecutor> logger)
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