using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HangfireServer
{
    public class EndpointTriggerer
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EndpointTriggerer> _logger;

        public EndpointTriggerer(IHttpClientFactory httpClientFactory, ILogger<EndpointTriggerer> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task Trigger(string url)
        {
            using var http = _httpClientFactory.CreateClient();
            try
            {
                http.Timeout = TimeSpan.FromSeconds(3);
                // fire and forget
                var res = await http.PostAsync(url, new StringContent(""));
            }
            catch (Exception e)
            {
                // we dont care 
            }

            _logger.LogInformation($"Sent request to {url}");
        }
    }
}