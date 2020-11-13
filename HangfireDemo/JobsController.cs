using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AbdusCo.CronJobs.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HangfireDemo
{
    [Route("/-/jobs")]
    internal class JobsController : Controller
    {
        private readonly IJobFactory _jobFactory;
        private readonly IEnumerable<IJobProvider> _jobProviders;
        private readonly ILogger<JobsController> _logger;

        public JobsController(IJobFactory jobFactory, IEnumerable<IJobProvider> jobProviders,
            ILogger<JobsController> logger)
        {
            _jobFactory = jobFactory;
            _jobProviders = jobProviders;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<JobDescription>>> GetTriggerableJobs()
        {
            return Json(_jobProviders.SelectMany(p => p.Jobs).ToList());
        }

        [HttpPost("{jobName}")]
        public async Task<IActionResult> ExecuteJob(string jobName, CancellationToken cancellationToken)
        {
            Response.OnCompleted(async () =>
            {
                var job = _jobFactory.Create(jobName);
                _logger.LogInformation($"Executing {job}");
                await job.ExecuteAsync(cancellationToken);
                _logger.LogInformation($"Finished {job}");
            });

            return Ok();
        }
    }
}