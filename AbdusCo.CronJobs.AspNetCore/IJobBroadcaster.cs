using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbdusCo.CronJobs.AspNetCore
{
    public interface IJobBroadcaster
    {
        Task BroadcastAsync(IEnumerable<JobDescription> jobs);
    }

    public sealed record JobBroadcast(string Application, string Environment, IEnumerable<JobDescription> Jobs);
}