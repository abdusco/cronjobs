namespace AbdusCo.CronJobs
{
    public class JobsOptions
    {
        public const string Key = "Jobs";
        public string RegistrationEndpoint { get; set; }
        public string UrlTemplate { get; set; }
        public int RetryCount { get; set; } = 5;
        public int WaitSeconds { get; set; } = 5;
        public int TimeoutSeconds { get; set; } = 60;
    }
}