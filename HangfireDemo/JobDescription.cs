namespace HangfireDemo
{
    public sealed record JobDescription(string Url, string Name, string Description, params string[] CronExpressions);
}