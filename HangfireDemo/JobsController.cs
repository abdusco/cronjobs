using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AbdusCo.CronJobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HangfireDemo
{
    [Route("/-/jobs")]
    internal class JobsController : Controller
    {
        private readonly ICronJobFactory _cronJobFactory;
        private readonly IEnumerable<ICronJobProvider> _jobProviders;
        private readonly ILogger<JobsController> _logger;

        public JobsController(ICronJobFactory cronJobFactory, IEnumerable<ICronJobProvider> jobProviders,
            ILogger<JobsController> logger)
        {
            _cronJobFactory = cronJobFactory;
            _jobProviders = jobProviders;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<CronJobDescription>>> GetTriggerableJobs()
        {
            return Json(_jobProviders.SelectMany(p => p.CronJobs).ToList());
        }

        [HttpPost("{jobName}")]
        public async Task<IActionResult> ExecuteJob(string jobName, CancellationToken cancellationToken)
        {
            Response.OnCompleted(async () =>
            {
                var job = _cronJobFactory.Create(jobName);
                _logger.LogInformation($"Executing {job}");
                await job.ExecuteAsync(cancellationToken);
                _logger.LogInformation($"Finished {job}");
            });

            return Ok();
        }
    }
}