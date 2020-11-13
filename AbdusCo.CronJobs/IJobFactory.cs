namespace AbdusCo.CronJobs
{
    public interface IJobFactory
    {
        IJob Create(string jobName);
    }
}