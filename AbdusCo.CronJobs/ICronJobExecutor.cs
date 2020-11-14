using System.Threading;
using System.Threading.Tasks;

namespace AbdusCo.CronJobs
{
    public interface ICronJobExecutor
    {
        Task ExecuteJobAsync(ICronJob cronJob, CancellationToken cancellationToken);
    }
}