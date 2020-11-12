using System.Collections.Generic;

namespace HangfireDemo.Providers
{
    public interface IJobProvider
    {
        IEnumerable<JobDescription> Jobs { get; }
    }
}