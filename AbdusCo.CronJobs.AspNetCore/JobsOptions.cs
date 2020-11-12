namespace AbdusCo.CronJobs.AspNetCore
{
    public class JobsOptions
    {
        public const string Key = "Jobs";
        public string RegistrationEndpoint { get; set; }
        public string UrlTemplate { get; set; }
    }
}