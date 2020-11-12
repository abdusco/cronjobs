using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbdusCo.CronJobs.Core
{
    public interface IJobBroadcaster
    {
        Task BroadcastAsync(params JobDescription[] jobs);
    }

    public sealed record JobBroadcast(string Application, string Environment, IEnumerable<JobDescription> Jobs);
}