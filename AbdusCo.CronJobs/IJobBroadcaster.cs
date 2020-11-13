using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AbdusCo.CronJobs
{
    public interface IJobBroadcaster
    {
        Task BroadcastAsync(IEnumerable<JobDescription> jobs, CancellationToken cancellationToken);
    }

    public class JobBroadcast
    {
        public string Application { get; set; }
        public string Environment { get; set; }

        public IEnumerable<JobDescription> Jobs { get; set; }
    }
}