using System.Collections.Generic;

namespace AbdusCo.CronJobs
{
    public interface IJobProvider
    {
        IEnumerable<JobDescription> Jobs { get; }
    }
}