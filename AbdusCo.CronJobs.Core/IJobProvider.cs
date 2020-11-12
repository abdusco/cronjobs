using System.Collections.Generic;

namespace AbdusCo.CronJobs.Core
{
    public interface IJobProvider
    {
        IEnumerable<JobDescription> Jobs { get; }
    }
}