using System.Threading;
using System.Threading.Tasks;

namespace AbdusCo.CronJobs
{
    public interface IJob
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}