namespace AbdusCo.CronJobs
{
    public interface ICronJobRegistry
    {
        void Register(CronJobBroadcast payload);
    }
}