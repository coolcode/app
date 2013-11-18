using System;
using EaseEasy.ServiceModel.Logging;
using Quartz;
using Quartz.Impl;

namespace Linkknil.Web {
    public class JobConfig {
        public static void RegisterJobs() {
            if (System.Configuration.ConfigurationManager.AppSettings["job-enabled"] != "true") {
                return;
            }
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
                .UsingJobData("type", "Linkknil.Jobs.CrawlJob,Linkknil.Services")
                .UsingJobData("method", "DigLinks")
                .Build();

            // fire every 5 mins
            var trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .StartAt(DateTimeOffset.Now.AddMinutes(1))
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(3).RepeatForever())
                .Build();

            sched.ScheduleJob(jobDetail, trigger);

        }

    }

    public class CommonJob : IJob {
        private static readonly ILogger logger = LogManager.GetLogger(typeof(CommonJob));

        public void Execute(IJobExecutionContext context) {
            var map = context.JobDetail.JobDataMap;
            var typeName = (string)map["type"];
            var methodName = (string)map["method"];

            logger.Info("开始执行:"+context.JobDetail.Description);

            try {
                var type = Type.GetType(typeName);
                var method = type.GetMethod(methodName);
                method.Invoke(Activator.CreateInstance(type), null);
            }
            catch (Exception ex) {
                logger.Error("执行任务发生异常，Job信息：[" + methodName + ":" + typeName + "]", ex);
            }
            finally
            {
                logger.Info("结束执行:" + context.JobDetail.Description);
            }
        }
    }
}