using System.Threading.Tasks;
using AbdusCo.CronJobs.Core;
using Microsoft.Extensions.Logging;

namespace HangfireDemo.Jobs
{
    [Cron("*/1 * * * *")]
    public class CreateReport : IJob
    {
        private readonly ILogger<CreateReport> _logger;

        public CreateReport(ILogger<CreateReport> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync()
        {
            _logger.LogInformation("creating report...");
            return Task.CompletedTask;
        }
    }
}