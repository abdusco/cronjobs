using System.Net.Http;
using System.Threading.Tasks;
using AbdusCo.CronJobs;
using Microsoft.Extensions.Logging;

namespace HangfireServer
{
    public class JobTriggerer
    {
        private readonly HttpClient _http;
        private readonly ILogger<JobTriggerer> _logger;

        public JobTriggerer(HttpClient http, ILogger<JobTriggerer> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task Trigger(CronJobDescription cronJob)
        {
            _logger.LogInformation("Triggering {Job}", cronJob);
            try
            {
                // fire and forget
                var res = await _http.PostAsync(cronJob.Endpoint, new StringContent(""));
            }
            catch (TaskCanceledException _)
            {
                // ignore timeout errors
            }

            _logger.LogInformation($"Sent request to {cronJob.Endpoint}");
        }
    }
}