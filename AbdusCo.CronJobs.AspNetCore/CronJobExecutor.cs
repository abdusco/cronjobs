using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AbdusCo.CronJobs.AspNetCore
{
    public class CronJobExecutor: IJobExecutor
    {
        private readonly ILogger<CronJobExecutor> _logger;

        public CronJobExecutor(ILogger<CronJobExecutor> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteJobAsync(IJob job, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Executing {job}");
            await job.ExecuteAsync(cancellationToken);
            _logger.LogInformation($"Finished {job}");
        }
    }
}