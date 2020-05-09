using System;
using System.Collections.Generic;
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
    public class DriverCarReportService :BaseService, IDriverReportBase, IDriverCarReportService
    {
        private readonly IMediator mediator;

        public DriverCarReportService(ApplicationDbContext context,IMediator mediator) : base(context)
        {
            this.mediator = mediator;
        }

        public DriverPeriodActivitiesDto DriverPeriodActivities { get; set; }
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
            DriverPeriodActivities = new DriverPeriodActivitiesDto();
            DriverPeriodActivities.TotalService = new TimeSpan();
            DriverPeriodActivities.TotalWork = new TimeSpan();
            DriverPeriodActivities.TotalAvailability = new TimeSpan();
            DriverPeriodActivities.TotalDrive = new TimeSpan();
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
                    DriverPeriodActivities.DailyActivities.Add(dayActivity);
            }

            var firstItem = dailyActivities.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name));
            if (firstItem != null)
                DriverPeriodActivities.FullName = firstItem.Name;
            DriverPeriodActivities.StartPeriod = startPeriod;
            DriverPeriodActivities.EndPeriod = endPeriod;
            await mediator.Publish(new ProgressNotification {Value = 80}).ConfigureAwait(false);
            CalculateCumulServices(DriverPeriodActivities);
            var detectionService = new DetectionOfDriverService();
            DriverPeriodActivities.DriverServices= detectionService.DetectionOfDriverServices(DriverPeriodActivities.DailyActivities);
            DetectionOfindemnities indemnities = new DetectionOfindemnities();
            indemnities.DriverPeriodActivities = DriverPeriodActivities;
            indemnities.DetectIndemnities();
            await mediator.Publish(new ProgressNotification {Value = 100}).ConfigureAwait(false);
            return DriverPeriodActivities;
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

        private static void CalculateCumulServices(DriverPeriodActivitiesDto DriverPeriodActivities)
        {
            foreach (var driverDailyActivityDto in DriverPeriodActivities.DailyActivities)
            {
                foreach (var activityDto in driverDailyActivityDto.Activities)
                    CountTotalServices(activityDto, driverDailyActivityDto);

                DriverPeriodActivities.TotalService += driverDailyActivityDto.TotalService;
                DriverPeriodActivities.TotalAvailability += driverDailyActivityDto.TotalAvailability;
                DriverPeriodActivities.TotalDrive += driverDailyActivityDto.TotalDrive;
                DriverPeriodActivities.TotalWork += driverDailyActivityDto.TotalWork;
                DriverPeriodActivities.TotalBreakRest += driverDailyActivityDto.TotalBreakRest;
                DriverPeriodActivities.TotalNightHour += driverDailyActivityDto.TotalNightHour;
            }
        }

        private static void CountTotalServices( ActivityDto activity, DriverDailyActivityDto DriverPeriodActivities)
        {
            switch (activity.ActivityType)
            {
                case DriverActivityType.Work:
                    DriverPeriodActivities.TotalWork += activity.EndUtc - activity.StartUtc;
                    DriverPeriodActivities.TotalService += activity.EndUtc - activity.StartUtc;
                    break;
                case DriverActivityType.Driving:
                    DriverPeriodActivities.TotalDrive += activity.EndUtc - activity.StartUtc;
                    DriverPeriodActivities.TotalService += activity.EndUtc - activity.StartUtc;
                    break;
                case DriverActivityType.Available:
                    DriverPeriodActivities.TotalAvailability += activity.EndUtc - activity.StartUtc;
                    DriverPeriodActivities.TotalService += activity.EndUtc - activity.StartUtc;
                    break;
                case DriverActivityType.Break:
                    DriverPeriodActivities.TotalBreakRest += activity.EndUtc - activity.StartUtc;
                    break;
            }
            // before 6H
            if (activity.StartUtc.ToLocalTime().TimeOfDay < new TimeSpan(6, 0, 0) && activity.ActivityType > 0 && (short)activity.ActivityType != 9)
            {
                if (activity.EndUtc.ToLocalTime().TimeOfDay > new TimeSpan(6, 0, 0))
                    DriverPeriodActivities.TotalNightHour += new TimeSpan(6, 0, 0) - activity.StartUtc.ToLocalTime().TimeOfDay;
                else
                    DriverPeriodActivities.TotalNightHour += activity.EndUtc.ToLocalTime().TimeOfDay - activity.StartUtc.ToLocalTime().TimeOfDay;
            }
            // after 21H
            if (activity.EndUtc.ToLocalTime().TimeOfDay > new TimeSpan(21, 0, 0) && activity.ActivityType > 0 && (short)activity.ActivityType != 9)
            {
                if (activity.StartUtc.ToLocalTime().TimeOfDay < new TimeSpan(21, 0, 0))
                    DriverPeriodActivities.TotalNightHour += activity.EndUtc.ToLocalTime().TimeOfDay - new TimeSpan(21, 0, 0);
                else
                    DriverPeriodActivities.TotalNightHour += activity.EndUtc.ToLocalTime().TimeOfDay - activity.StartUtc.ToLocalTime().TimeOfDay;
            }
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
