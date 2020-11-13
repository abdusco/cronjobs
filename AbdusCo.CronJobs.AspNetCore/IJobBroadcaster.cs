using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AbdusCo.CronJobs.AspNetCore
{
    public interface IJobBroadcaster
    {
        Task BroadcastAsync(IEnumerable<JobDescription> jobs, CancellationToken cancellationToken);
    }

    public sealed record JobBroadcast(string Application, string Environment, IEnumerable<JobDescription> Jobs);
}