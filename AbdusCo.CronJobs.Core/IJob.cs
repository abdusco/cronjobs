using System.Threading.Tasks;

namespace AbdusCo.CronJobs.Core
{
    public interface IJob
    {
        Task ExecuteAsync();
    }
}