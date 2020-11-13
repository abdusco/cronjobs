using System.Threading;
using System.Threading.Tasks;

namespace AbdusCo.CronJobs
{
    public interface IJobExecutor
    {
        Task ExecuteJobAsync(IJob job, CancellationToken cancellationToken);
    }
}