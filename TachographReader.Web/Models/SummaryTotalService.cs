using System;
using TachographReader.Application.Dtos.Activities;

namespace TachographReader.Web.Models
{
    internal class SummaryTotalService
    {
        public string TotalService { get; set; }
        public string TotalAvailability { get; set; }
        public string TotalWork { get; set; }
        public string TotalDrive { get; set; }
        public string TotalNightHour { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public string DriverName { get; set; }
        public static SummaryTotalService GetTotalService(DriverPeriodActivitiesDto report)
        {
            var totalService = new SummaryTotalService
            {
                StartPeriod = report.StartPeriod,
                EndPeriod = report.EndPeriod,
                DriverName = report.FullName,
                TotalService = string.Format("{0}H{1:00}",
                    report.TotalService.Days * 24 +
                    report.TotalService.Hours,
                    report.TotalService.Minutes),
                TotalAvailability = string.Format("{0}H{1:00}",
                    report.TotalAvailability.Days * 24 +
                    report.TotalAvailability.Hours,
                    report.TotalAvailability.Minutes),
                TotalWork = string.Format("{0}H{1:00}",
                    report.TotalWork.Days * 24 + report.TotalWork.Hours,
                    report.TotalWork.Minutes),
                TotalDrive = string.Format("{0}H{1:00}",
                    report.TotalDrive.Days * 24 +
                    report.TotalDrive.Hours, report.TotalDrive.Minutes),
                TotalNightHour = string.Format("{0}H{1:00}",
                    report.TotalNightHour.Days * 24 +
                    report.TotalNightHour.Hours,
                    report.TotalNightHour.Minutes),
            };
            return totalService;
        }

    }
}