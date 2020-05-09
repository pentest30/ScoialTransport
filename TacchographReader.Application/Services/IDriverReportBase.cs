using System;
using System.Threading.Tasks;
using TachographReader.Application.Dtos.Activities;

namespace TachographReader.Application.Services
{
    public interface IDriverReportBase
    {
        public DriverPeriodActivitiesDto DriverPeriodActivities { get; set; }
        
    }
}
