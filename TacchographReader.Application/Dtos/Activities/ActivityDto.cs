using System;
using tacchograaph_reader.Core.Entities;

namespace TachographReader.Application.Dtos.Activities
{
    public class ActivityDto : ICloneable
    {
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public DriverActivityType ActivityType { get; set; }
        public double Duration { get; set; }
        public string DriverName { get; set; }
        public string CardNumber { get; set; }
        public object Clone()
        {
            ActivityDto clone = new ActivityDto();
            clone.StartUtc = this.StartUtc;
            clone.EndUtc = this.EndUtc;
            clone.ActivityType = this.ActivityType;
            clone.Duration = this.Duration;
            clone.DriverName = this.DriverName;
            clone.CardNumber = this.CardNumber;
            return clone;
        }
    }
}