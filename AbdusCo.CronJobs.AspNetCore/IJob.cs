using System.Threading.Tasks;

namespace AbdusCo.CronJobs.AspNetCore
{
    public interface IJob
    {
        Task ExecuteAsync();
    }
}