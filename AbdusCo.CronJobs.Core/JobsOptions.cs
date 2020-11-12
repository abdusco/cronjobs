namespace AbdusCo.CronJobs.Core
{
    public class JobsOptions
    {
        public const string Key = "Jobs";
        public string RegistrationEndpoint { get; set; }
        public int Timeout { get; set; } = 5;
    }
}