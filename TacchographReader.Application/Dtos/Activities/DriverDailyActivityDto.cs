using System;
using System.Collections.Generic;

namespace TachographReader.Application.Dtos.Activities
{
    public class DriverDailyActivityDto
    {
        public DriverDailyActivityDto()
        {
            Activities = new List<ActivityDto>();
            Id = Guid.NewGuid();
        }
        public DateTime Date { get; set; }
        public TimeSpan TotalService { get; set; }
        public TimeSpan TotalDrive { get; set; }
        public TimeSpan TotalWork { get; set; }
        public TimeSpan TotalAvailability { get; set; }
        public double TotalDistance { get; set; }
        public List<ActivityDto> Activities { get; set; }
        public TimeSpan TotalBreakRest { get; set; }
        public TimeSpan TotalNightHour { get; set; }
        public Guid Id { get; set; }
        
    }
}
