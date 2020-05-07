using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using tacchograaph_reader.Core.Commands.DddFiles;
using tacchograaph_reader.Core.Entities;
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
            var dailyActivities = await (from identifier in Context.Identifiers
                    from activity in Context.CardDriverActivities
                    join dailyAct in Context.CardActivityDailyRecords on identifier.CardNumber equals dailyAct
                        .CardNumber
                    from driver in Context.Drivers where driver.Id == driverId
                    where identifier.DriverId == driverId && activity.ActivityDailyRecordId == dailyAct.Id &&
                          activity.ActivityUtc >= startPeriod && activity.ActivityUtc <= endPeriod
                    orderby dailyAct.Date
                    select new
                    {
                        driver.Name,
                        dailyAct.CardNumber,
                        dailyAct.Date,
                        dailyAct.TotalDistance,
                        activity.TimeSpan,
                        activity.CardPresent,
                        activity.DriverActivityType,
                        activity.ActivityUtc
                    }).Distinct().ToListAsync()
                .ConfigureAwait(false);
            await mediator.Publish(new ProgressNotification { Value = 50}).ConfigureAwait(false);
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
               if(dayActivity.Activities.Any())
                   report.DailyActivities.Add(dayActivity);
            }

            var firstItem = dailyActivities.FirstOrDefault(x=>!string.IsNullOrEmpty(x.Name));
            if (firstItem != null)
                report.FullName = firstItem.Name;
            report.StartPeriod = startPeriod;
            report.EndPeriod = endPeriod;
            await mediator.Publish(new ProgressNotification { Value = 80 }).ConfigureAwait(false);
            CalculateCumulServices(report);
            await mediator.Publish(new ProgressNotification { Value = 100 }).ConfigureAwait(false);

            return report;
        }

        private static void SetDurationsOfActivities(DriverDailyActivityDto dayActivity)
        {
            foreach (var dayActivityActivity in dayActivity.Activities.OrderBy(x => x.StartUtc))
            {
                Console.WriteLine("start: " + dayActivityActivity.StartUtc + " end: " + dayActivityActivity.EndUtc);
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
     
     
    }
}
