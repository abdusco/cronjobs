namespace HangfireDemo
{
    public class HangfireConfig
    {
        public const string Key = "Jobs";
        public string RegistrationEndpoint { get; set; }
        public int Timeout { get; set; } = 5;
    }
}