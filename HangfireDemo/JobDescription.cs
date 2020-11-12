using System.Collections.Generic;

namespace HangfireDemo
{
    public sealed record JobDescription
    {
        public string Name { get; init; }
        public string? Description { get; init; }
        public IEnumerable<string> CronExpressions { get; init; }

        public JobDescription(string name, params string[] cronExpressions)
        {
            Name = name;
            CronExpressions = cronExpressions;
        }
    }
}