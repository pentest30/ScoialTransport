using System;
using System.Security;
using tacchograaph_reader.Core.Entities;

namespace TachographReader.Application.Dtos.Activities
{
    public class ActivityDto
    {
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public DriverActivityType ActivityType { get; set; }
        public double Duration { get; set; }
        public string DriverName { get; set; }
        public string CardNumber { get; set; }
    }
}