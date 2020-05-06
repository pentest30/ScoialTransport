using System;
using System.Collections.Generic;

namespace TachographReader.Application.Dtos.Activities
{
    public class DriverPeriodActivitiesDto
    {
        public DriverPeriodActivitiesDto()
        {
            DailyActivities = new List<DriverDailyActivityDto>();
        }
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public string FullName { get; set; }
        public TimeSpan TotalService { get; set; }
        public TimeSpan TotalDrive { get; set; }
        public TimeSpan TotalWork { get; set; }
        public TimeSpan TotalAvailability { get; set; }

        public TimeSpan TotalBreakRest { get; set; }
        public TimeSpan TotalNightHour { get; set; }
        public List<DriverDailyActivityDto> DailyActivities { get; set; }
    }
}
