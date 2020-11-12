using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HangfireDemo.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HangfireDemo
{
    [Route("/-/jobs")]
    public class JobsController : Controller
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
        public async Task<IActionResult> ExecuteJob(string jobName)
        {
            var job = _jobFactory.Create(jobName);
            if (job == null)
            {
                return NotFound();
            }

            _logger.LogInformation($"Executing {job}");
            await job.ExecuteAsync();
            _logger.LogInformation($"Finished {job}");

            return Ok();
        }
    }
}