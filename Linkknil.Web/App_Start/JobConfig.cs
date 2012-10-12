using System;
using Quartz;
using Quartz.Impl;

namespace Linkknil.Web {
    public class JobConfig {
        public static void RegisterJobs() {
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();

            // construct job info
            IJobDetail jobDetail = JobBuilder.Create()
                .WithIdentity("PullUrlJob", "jobs")
                .OfType(typeof(CommonJob))
                .WithDescription("内容源抓取服务")
                .UsingJobData("type", "Linkknil.Services.LinkService,Linkknil.Services")
                .UsingJobData("method", "DigLinks")
                .Build();

            // fire every 5 mins
            var trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .StartAt(DateTimeOffset.Now.AddMinutes(1))
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(5).RepeatForever())
                .Build();

            sched.ScheduleJob(jobDetail, trigger);

        }

    }

    public class CommonJob : IJob {
        public void Execute(IJobExecutionContext context) {
            var map = context.JobDetail.JobDataMap;
            var typeName = (string)map["type"];
            var methodName = (string)map["method"];
            var type = Type.GetType(typeName);
            var method = type.GetMethod(methodName);
            method.Invoke(Activator.CreateInstance(type), null);
        }
    }
}