using System;
using System.Collections.Generic;

namespace TachographReader.Application.Dtos.Activities
{
    public class DriverService
    {
        public BreakBeforeService BreakBeforeService { get; set; }
        public BreakAfterService BreakAfterService { get; set; }
        public DateTime BeginningServiceTime { get; set; }
        public DateTime EndingBServiceTime { get; set; }
        public List<ActivityDto> Activities { get; set; }
    }
}