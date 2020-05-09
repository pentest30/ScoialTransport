using System;
using System.Collections.Generic;
using System.Linq;
using tacchograaph_reader.Core.Entities;
using tacchograaph_reader.Core.Extensions;
using TachographReader.Application.Dtos.Activities;

namespace TachographReader.Application.Services
{
    public class DetectionOfDriverService
    {
       
        public List<DriverService> DetectionOfDriverServices(List<DriverDailyActivityDto> driverPeriodActivities)
        {
            var periods = new List<Period>();
            var listOfAllActivities = new List<ActivityDto>();
            var dailyActs = new List<DriverDailyActivityDto>(driverPeriodActivities);
            var driverServices = new List<DriverService>();
            foreach (var driverDailyActivityDto in dailyActs)
            {
                var activities = (List<ActivityDto>)driverDailyActivityDto.Activities.Clone();
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
                driverService.TotalDistance =(int) driverPeriodActivities
                    .FirstOrDefault(x => x.Date == driverService.BeginningServiceTime.Date).TotalDistance;
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
        private static List<ActivityDto> MergeSameActivities(List<ActivityDto> c1BActivityList)
        {
            var result = new List<ActivityDto>();
            var removeList = new List<ActivityDto>();
            var fistAct = default(ActivityDto);
            foreach (var activityDto in c1BActivityList)
            {
                if (fistAct != default && (Math.Abs((activityDto.StartUtc - fistAct.EndUtc).TotalSeconds) <= 0 || fistAct.EndUtc.AddDays(1).Date == activityDto.StartUtc) && fistAct.ActivityType == activityDto.ActivityType)
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
}
