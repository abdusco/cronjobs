namespace AbdusCo.CronJobs
{
    public interface ICronJobFactory
    {
        ICronJob Create(string jobName);
    }
}