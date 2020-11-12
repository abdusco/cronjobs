using System.Threading.Tasks;

namespace HangfireDemo.Jobs
{
    public interface IJob
    {
        Task ExecuteAsync();
    }
}