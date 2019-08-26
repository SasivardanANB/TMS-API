using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;

namespace TMS.BusinessGateway.Classes
{
    public class EmailJobScheduler
    {
        public async static void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();
            IJobDetail job = JobBuilder.Create<EmailJob>().Build();
            //IJobDetail job = JobBuilder.Create();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["ReadEmailsIntervel"]))
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                  )
                .Build();
            await scheduler.ScheduleJob(job, trigger);
        }

    }
}
