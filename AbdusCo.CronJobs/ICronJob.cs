using System.Threading;
using System.Threading.Tasks;

namespace AbdusCo.CronJobs
{
    public interface ICronJob
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}