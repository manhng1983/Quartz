using System;
using System.Text;
using log4net;
using Quartz;
using Quartz.Impl;

namespace OOP.Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Scheduler scheduler = new Scheduler();
            if (!scheduler.CheckScheduleStart())
            {
                if (int.TryParse(System.Configuration.ConfigurationManager.AppSettings["EveryDayAtHour"], out int hour)
                    && int.TryParse(System.Configuration.ConfigurationManager.AppSettings["EveryDayAtMinute"], out int minute))
                {
                    scheduler.Start(hour, minute);
                    Logger.Log.Info("Bắt đầu.");
                    Console.WriteLine("Bắt đầu.");
                }
            }
            else
            {
                scheduler.Stop();
                Logger.Log.Info("Kết thúc.");
                Console.WriteLine("Kết thúc.");
            }
        }
    }

    class Job : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Log.Info("Thực hiện Job và kết thúc");
                Console.WriteLine("Thực hiện Job và kết thúc");
                System.Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                Console.WriteLine(ex.ToString());
            }
        }
    }

    class Scheduler
    {
        IScheduler scheduler;
        IJobDetail job;
        ITrigger trigger;

        public void Start(int hour, int minute)
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            job = JobBuilder.Create<Job>().Build();

            trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minute))
                  )
                .Build();
            scheduler.ScheduleJob(job, trigger);
        }

        public bool CheckScheduleStart()
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            return scheduler.IsStarted;
        }

        public void Stop()
        {
            if (scheduler.IsStarted)
            {
                scheduler.Shutdown();
            }
        }
    }

    public class Logger
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

        public static ILog Log
        {
            get { return Logger.log; }
        }
    }
}
