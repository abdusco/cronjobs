using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AbdusCo.CronJobs;
using Microsoft.Extensions.Logging;

namespace HangfireDemo.Jobs
{
    [Cron("*/1 * * * *", "1 1 * * *")]
    public class CreateReport : ICronJob
    {
        private readonly ILogger<CreateReport> _logger;
        private readonly DemoDbContext _db;

        public CreateReport(ILogger<CreateReport> logger, DemoDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("creating report...");
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            var sum = _db.Sales.Sum(s => s.Total);
            _logger.LogInformation($"created report. sum = {sum}");
        }
    }
}