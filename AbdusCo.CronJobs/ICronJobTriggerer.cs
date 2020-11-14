using System.Threading.Tasks;

namespace AbdusCo.CronJobs
{
    public interface ICronJobTriggerer
    {
        Task Trigger(CronJobDescription cronJob);
    }
}