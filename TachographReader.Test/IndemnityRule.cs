using System;
using System.Collections.Generic;
using System.Text;

namespace TachographReader.Test
{
    public class IndemnityRule
    {
        public TimeSpan StarTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan? SecondStarTime { get; set; }
        public TimeSpan? SecondEndTime { get; set; }
        public float Price { get; set; }
        public string Label { get; set; }
    }
}
