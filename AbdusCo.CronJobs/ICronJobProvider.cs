using System.Collections.Generic;

namespace AbdusCo.CronJobs
{
    public interface ICronJobProvider
    {
        IEnumerable<CronJobDescription> CronJobs { get; }
    }
}