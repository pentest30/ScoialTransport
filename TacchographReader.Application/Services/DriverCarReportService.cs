using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using tacchograaph_reader.Core.Commands.DddFiles;
using tacchograaph_reader.Core.Entities;
using tacchograaph_reader.Core.Extensions;
using TachographReader.Application.Dtos.Activities;
using TachoReader.Data.Data;

namespace TachographReader.Application.Services
{
    public class DriverCarReportService :BaseService, IDriverCarReportService
    {
        private readonly IMediator mediator;

        public DriverCarReportService(ApplicationDbContext context,IMediator mediator) : base(context)
        {
            this.mediator = mediator;
        }

        public async Task<DriverPeriodActivitiesDto> GetDriverCardReportFromByPeriodAsync(Guid driverId, DateTime start,
            DateTime end)
        {
            var startPeriod = start.ToUniversalTime();
            var endPeriod = end.ToUniversalTime();
            var dailyActivities = await DailyActivities(driverId, startPeriod, endPeriod).ConfigureAwait(false);
            var lastActivity = dailyActivities.FirstOrDefault();
            if (lastActivity != null)
            {
                var previousAct = await GetPreviousActivityAsync(driverId, startPeriod, lastActivity.CardNumber)
                    .ConfigureAwait(false);
                if (previousAct != null)
                    dailyActivities.Add(previousAct);
            }

            await mediator.Publish(new ProgressNotification {Value = 50}).ConfigureAwait(false);
            var activitiesDates = dailyActivities
                .Select(x => x.Date)
                .Distinct()
                .OrderBy(x => x.Date)
                .ToList();
            var report = new DriverPeriodActivitiesDto();
            report.TotalService = new TimeSpan();
            report.TotalWork = new TimeSpan();
            report.TotalAvailability = new TimeSpan();
            report.TotalDrive = new TimeSpan();
            foreach (var activitiesDate in activitiesDates)
            {
                var activities = dailyActivities.Where(x => x.Date == activitiesDate).ToList();
                var dayActivity = new DriverDailyActivityDto();
                foreach (var activity in activities.OrderBy(x => x.ActivityUtc))
                {
                    var index = activities.IndexOf(activity);
                    var actDto = new ActivityDto();
                    actDto.StartUtc = activity.ActivityUtc;
                    if (activity.CardPresent)
                    {
                        actDto.CardNumber = activity.CardNumber;
                        actDto.DriverName = activity.Name;
                    }

                    actDto.ActivityType = activity.DriverActivityType;
                    dayActivity.TotalDistance = activity.TotalDistance;
                    dayActivity.Activities.Add(actDto);

                }

                dayActivity.Date = activitiesDate;


                dayActivity.Activities = dayActivity.Activities.OrderBy(x => x.StartUtc).ToList();
                for (int i = 0; i < dayActivity.Activities.Count; i++)
                    if (i + 1 < dayActivity.Activities.Count)
                        dayActivity.Activities[i].EndUtc = dayActivity.Activities[i + 1].StartUtc;


                var lastAct = dayActivity.Activities.LastOrDefault();
                if (lastAct != null)
                {
                    var index = dayActivity.Activities.IndexOf(lastAct);
                    lastAct.EndUtc = activitiesDate.Date.AddDays(1).ToUniversalTime();
                    dayActivity.Activities[index] = lastAct;
                }

                SetDurationsOfActivities(dayActivity);
                if (dayActivity.Activities.Any())
                    report.DailyActivities.Add(dayActivity);
            }

            var firstItem = dailyActivities.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name));
            if (firstItem != null)
                report.FullName = firstItem.Name;
            report.StartPeriod = startPeriod;
            report.EndPeriod = endPeriod;
            await mediator.Publish(new ProgressNotification {Value = 80}).ConfigureAwait(false);
            CalculateCumulServices(report);
            report.DriverServices= DetectionOfDriverServices(report.DailyActivities);
            await mediator.Publish(new ProgressNotification {Value = 100}).ConfigureAwait(false);
            return report;
        }

        private  List<DriverService> DetectionOfDriverServices(List<DriverDailyActivityDto> report)
        {
            var periods = new List<Period>();
            var listOfAllActivities = new List<ActivityDto>();
            var dailyActs = new List<DriverDailyActivityDto>(report);
            var driverServices = new List<DriverService>();
            foreach (var driverDailyActivityDto in dailyActs)
            {
                var activities = new List<ActivityDto>();
                activities =(List<ActivityDto>) driverDailyActivityDto.Activities.Clone();
                var fActivity = activities.OrderBy(x => x.StartUtc).FirstOrDefault();
                // if the first  activity with break rest type does not begin from 00 :00 
                if (fActivity != null && fActivity.ActivityType == DriverActivityType.Break)
                    fActivity.StartUtc = fActivity.StartUtc.Date;
                // add all daily activities in one list in order to merge the same activities
                listOfAllActivities.AddRange(activities);
            }

            // merges same activities
            var mergedList = MergeSameActivities(listOfAllActivities.OrderBy(x => x.StartUtc).ToList());
            // detects long breaks after or befor the service (break > 7 hours)  
            periods.AddRange(CutC1BActivities(mergedList));

           
            foreach (var period in periods.OrderBy(x => x.Start))
                CreateListOfService(period, driverServices);

            foreach (var driverService in driverServices)
            {
                if (driverService.BreakAfterService == null || driverService.BreakBeforeService == null) continue;
                driverService.BeginningServiceTime = driverService.BreakBeforeService.EndingBServiceTime;
                driverService.EndingBServiceTime = driverService.BreakAfterService.BeginningBreakTime;
            }

            return driverServices;
        }

        private static void CreateListOfService(Period period, List<DriverService> driverServices)
        {

            if (driverServices.Any())
            {
                driverServices.Last().BreakAfterService = new BreakAfterService
                {
                    BeginningBreakTime = period.Start,
                    EndingBServiceTime = period.Start.Date != period.End.Date
                        ? period.End.Date.AddTicks(-1)
                        : period.End,
                };
            }

            // two service of different days
            if (period.Start.Date != period.End.Date)
            {
                driverServices.Add(new DriverService
                {
                    BreakBeforeService = new BreakBeforeService
                    {
                        BeginningBreakTime = period.Start,
                        EndingBServiceTime = period.End.Date.AddTicks(-1),
                    }
                });
                driverServices.Add(new DriverService
                {
                    BreakBeforeService = new BreakBeforeService
                    {
                        BeginningBreakTime = period.End.Date,
                        EndingBServiceTime = period.End,
                    }
                });
            }
            else
            {
                driverServices.Add(new DriverService
                {
                    BreakBeforeService = new BreakBeforeService
                    {
                        BeginningBreakTime = period.Start,
                        EndingBServiceTime = period.End,
                    }
                });
            }
        }

        private async Task<List<ActivityQuery>> DailyActivities(Guid driverId, DateTime startPeriod, DateTime endPeriod)
        {
            var dailyActivities = await (from identifier in Context.Identifiers
                    from activity in Context.CardDriverActivities
                    join dailyAct in Context.CardActivityDailyRecords on identifier.CardNumber equals dailyAct
                        .CardNumber
                    from driver in Context.Drivers
                    where driver.Id == driverId
                    where identifier.DriverId == driverId && activity.ActivityDailyRecordId == dailyAct.Id &&
                          activity.ActivityUtc >= startPeriod && activity.ActivityUtc <= endPeriod
                    orderby dailyAct.Date
                    select new ActivityQuery
                    {
                        Name = driver.Name,
                        CardNumber = dailyAct.CardNumber,
                        Date = dailyAct.Date,
                        TotalDistance = dailyAct.TotalDistance,
                        TimeSpan = activity.TimeSpan,
                        CardPresent = activity.CardPresent,
                        DriverActivityType = activity.DriverActivityType,
                        ActivityUtc = activity.ActivityUtc
                    }).Distinct().ToListAsync()
                .ConfigureAwait(false);
            return dailyActivities;
        }

        private async Task<ActivityQuery> GetPreviousActivityAsync ( Guid driverId, DateTime date , string cardNumber)
        {
            var startPeriod = date.Date;
            var query = await (from identifier in Context.Identifiers
                from activity in Context.CardDriverActivities
                join dailyAct in Context.CardActivityDailyRecords on identifier.CardNumber equals dailyAct
                    .CardNumber
                from driver in Context.Drivers
                where driver.Id == driverId
                where identifier.DriverId == driverId && activity.ActivityDailyRecordId == dailyAct.Id &&
                      activity.ActivityUtc > startPeriod && activity.ActivityUtc < date
                orderby activity.ActivityUtc descending
                select new ActivityQuery
                {
                    Name = driver.Name,
                    CardNumber = dailyAct.CardNumber,
                    Date = dailyAct.Date,
                    TotalDistance = dailyAct.TotalDistance,
                    TimeSpan = activity.TimeSpan,
                    CardPresent = activity.CardPresent,
                    DriverActivityType = activity.DriverActivityType,
                    ActivityUtc = activity.ActivityUtc
                }).FirstOrDefaultAsync().ConfigureAwait(false);
            return query;
        }
        private static void SetDurationsOfActivities(DriverDailyActivityDto dayActivity)
        {
            foreach (var dayActivityActivity in dayActivity.Activities.OrderBy(x => x.StartUtc))
            {
                //Console.WriteLine("start: " + dayActivityActivity.StartUtc + " end: " + dayActivityActivity.EndUtc);
                dayActivityActivity.Duration = (dayActivityActivity.EndUtc - dayActivityActivity.StartUtc).TotalSeconds;
            }
        }

        private static void CalculateCumulServices(DriverPeriodActivitiesDto report)
        {
            foreach (var driverDailyActivityDto in report.DailyActivities)
            {
                foreach (var activityDto in driverDailyActivityDto.Activities)
                    CountTotalServices(activityDto, driverDailyActivityDto);

                report.TotalService += driverDailyActivityDto.TotalService;
                report.TotalAvailability += driverDailyActivityDto.TotalAvailability;
                report.TotalDrive += driverDailyActivityDto.TotalDrive;
                report.TotalWork += driverDailyActivityDto.TotalWork;
                report.TotalBreakRest += driverDailyActivityDto.TotalBreakRest;
                report.TotalNightHour += driverDailyActivityDto.TotalNightHour;
            }
        }

        private static void CountTotalServices( ActivityDto activity, DriverDailyActivityDto report)
        {
            switch (activity.ActivityType)
            {
                case DriverActivityType.Work:
                    report.TotalWork += activity.EndUtc - activity.StartUtc;
                    report.TotalService += activity.EndUtc - activity.StartUtc;
                    break;
                case DriverActivityType.Driving:
                    report.TotalDrive += activity.EndUtc - activity.StartUtc;
                    report.TotalService += activity.EndUtc - activity.StartUtc;
                    break;
                case DriverActivityType.Available:
                    report.TotalAvailability += activity.EndUtc - activity.StartUtc;
                    report.TotalService += activity.EndUtc - activity.StartUtc;
                    break;
                case DriverActivityType.Break:
                    report.TotalBreakRest += activity.EndUtc - activity.StartUtc;
                    break;
            }
            // before 6H
            if (activity.StartUtc.ToLocalTime().TimeOfDay < new TimeSpan(6, 0, 0) && activity.ActivityType > 0 && (short)activity.ActivityType != 9)
            {
                if (activity.EndUtc.ToLocalTime().TimeOfDay > new TimeSpan(6, 0, 0))
                    report.TotalNightHour += new TimeSpan(6, 0, 0) - activity.StartUtc.ToLocalTime().TimeOfDay;
                else
                    report.TotalNightHour += activity.EndUtc.ToLocalTime().TimeOfDay - activity.StartUtc.ToLocalTime().TimeOfDay;
            }
            // after 21H
            if (activity.EndUtc.ToLocalTime().TimeOfDay > new TimeSpan(21, 0, 0) && activity.ActivityType > 0 && (short)activity.ActivityType != 9)
            {
                if (activity.StartUtc.ToLocalTime().TimeOfDay < new TimeSpan(21, 0, 0))
                    report.TotalNightHour += activity.EndUtc.ToLocalTime().TimeOfDay - new TimeSpan(21, 0, 0);
                else
                    report.TotalNightHour += activity.EndUtc.ToLocalTime().TimeOfDay - activity.StartUtc.ToLocalTime().TimeOfDay;
            }
        }

        private static List<ActivityDto> MergeSameActivities(List<ActivityDto> c1BActivityList)
        {
            var result = new List<ActivityDto>();
            var removeList = new List<ActivityDto>();
            var fistAct = default(ActivityDto);
            foreach (var activityDto in c1BActivityList)
            {
                if (fistAct!= default&& (Math.Abs((activityDto.StartUtc- fistAct.EndUtc).TotalSeconds) <= 0 ||fistAct.EndUtc.AddDays(1).Date  == activityDto.StartUtc )&& fistAct.ActivityType == activityDto.ActivityType)
                {
                    fistAct.EndUtc = activityDto.EndUtc;
                    removeList.Add(activityDto);
                }
                else fistAct = activityDto;
            }
           
            foreach (var activityDto in removeList)
                c1BActivityList.Remove(activityDto);
            result.AddRange(c1BActivityList);
            return c1BActivityList;

        }
        private static List<Period> CutC1BActivities(List<ActivityDto> c1BActivityList)
        {
            var periods = new List<Period>();
            foreach (var activity in c1BActivityList.OrderBy(x => x.StartUtc))
            {
                if ((activity.EndUtc - activity.StartUtc).TotalHours > 7 && activity.ActivityType == DriverActivityType.Break)
                {
                   periods.Add(new Period
                    {
                        Start = activity.StartUtc,
                        ActivityType = activity.ActivityType,
                        End = activity.EndUtc
                    });
                }
            }

            foreach (var period in periods)
            {
                Console.WriteLine("Period  start: " + period.Start + " end: " + period.End);
            }
            return periods;
        }


    }

    internal class Period
    {
        //public DateTime Date { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public DriverActivityType ActivityType { get; set; }
     
    }
    internal class ActivityQuery
    {
        public DateTime Date;
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public double TotalDistance { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public bool CardPresent { get; set; }
        public DriverActivityType DriverActivityType { get; set; }
        public DateTime ActivityUtc { get; set; }
    }
}
