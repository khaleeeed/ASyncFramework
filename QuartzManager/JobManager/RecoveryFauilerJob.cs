using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzManager.JobManager
{
    [DisallowConcurrentExecution]
    public class RecoveryFauilerJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Hello world");
            return Task.CompletedTask;
        }
    }
}