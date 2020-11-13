using System.Threading;
using System.Threading.Tasks;

namespace AbdusCo.CronJobs.AspNetCore
{
    public interface IJob
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}