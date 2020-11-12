using System.Collections.Generic;
using System.Linq;

namespace AbdusCo.CronJobs.AspNetCore
{
    public sealed record JobDescription
    {
        public string Name { get; init; }
        public string Endpoint { get; init; }
        public List<string> CronExpressions { get; init; }
        public string? Description { get; init; }
    }
}