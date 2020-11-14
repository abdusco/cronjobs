using System;
using System.Linq;
using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using AbdusCo.CronJobs;

namespace AbdusCo.CronJobs.Hangfire
{
    public class CronJobRegistry : ICronJobRegistry
    {
        private readonly IRecurringJobManager _jobManager;
        private readonly JobStorage _jobStorage;
        private readonly ILogger<CronJobRegistry> _logger;

        public CronJobRegistry(IRecurringJobManager jobManager, JobStorage jobStorage, ILogger<CronJobRegistry> logger)
        {
            _jobManager = jobManager;
            _jobStorage = jobStorage;
            _logger = logger;
        }

        public void Register(CronJobBroadcast payload)
        {
            string MakeKey(CronJobDescription j, int i) => $"{payload.Application}@{payload.Environment}.{j.Name}#{i}";

            var app = $"{payload.Application}@{payload.Environment}";
            _logger.LogDebug("Looking for registered jobs for {Application}", app);
            var existingJobs = _jobStorage.GetConnection().GetRecurringJobs()
                .Where(j => j.Id.StartsWith(app))
                .ToList();

            _logger.LogDebug($"Found {existingJobs.Count} jobs. Replacing with new ones.");

            foreach (var job in existingJobs)
            {
                _jobManager.RemoveIfExists(job.Id);
            }

            var jobs = payload.Jobs.ToList();
            foreach (var job in jobs)
            {
                _logger.LogDebug("Registering {Job} for {Application}", job, payload.Application);
                for (var i = 0; i < job.CronExpressions.Count; i++)
                {
                    var cron = job.CronExpressions[i];
                    var jobKey = MakeKey(job, i);
                    _jobManager.AddOrUpdate<CronJobTriggerer>(
                        jobKey,
                        t => t.Trigger(job),
                        cron,
                        TimeZoneInfo.Local
                    );
                }
            }

            _logger.LogInformation($"Registered {jobs.Count} jobs for {{Application}}.", app);
        }
    }
}