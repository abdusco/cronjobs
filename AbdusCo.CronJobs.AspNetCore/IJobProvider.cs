using System.Collections.Generic;

namespace AbdusCo.CronJobs.AspNetCore
{
    public interface IJobProvider
    {
        IEnumerable<JobDescription> Jobs { get; }
    }
}