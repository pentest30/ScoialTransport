using System;
using System.Collections.Generic;

namespace TachographReader.Application.Dtos.Driver
{
    public class IndemnityRule
    {
        public List<IndemnityInterval> IndemnityIntervals { get; set; }
        public float Price { get; set; }
        public string Label { get; set; }
        public bool? TotalDistanceRequired { get; set; }
        public int? MaxTotalDistance { get; set; }
        public int? MinToTalHours { get; set; }
    }

    public class IndemnityInterval
    {
        public TimeSpan? StarTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
