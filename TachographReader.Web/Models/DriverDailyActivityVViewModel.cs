using System;
using System.Collections.Generic;
using System.Globalization;
using TachographReader.Application.Dtos.Activities;
using TachographReader.Web.Helpers;

namespace TachographReader.Web.Models
{
    public class DriverDailyActivityVViewModel
    {
        public DriverDailyActivityVViewModel(DriverDailyActivityDto report)
        {
            Id = report.Id.ToString();
            Activities = report.Activities;
            WeekNumber = $"Week  { DateHelper.GetWeekNumberOfMonth(report.Date)} of {CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(report.Date.Month)} {report.Date.Year}";
            Activities.ForEach(x=>
            {
                x.EndUtc = x.EndUtc.ToLocalTime();
                x.StartUtc = x.StartUtc.ToLocalTime();
               
            });

            Date = report.Date;
            TotalDistance = report.TotalDistance;
            TotalService =
                $"{report.TotalService.Days * 24 + report.TotalService.Hours}H{report.TotalService.Minutes:00}";
            TotalAvailability =
                $"{report.TotalAvailability.Days * 24 + report.TotalAvailability.Hours}H{report.TotalAvailability.Minutes:00}";
            TotalWork = $"{report.TotalWork.Days * 24 + report.TotalWork.Hours}H{report.TotalWork.Minutes:00}";
            TotalDrive = $"{report.TotalDrive.Days * 24 + report.TotalDrive.Hours}H{report.TotalDrive.Minutes:00}";
            TotalNightHour =
                $"{report.TotalNightHour.Days * 24 + report.TotalNightHour.Hours}H{report.TotalNightHour.Minutes:00}";
        }
        public DateTime Date { get; set; }
        public string TotalService { get; set; }
        public string TotalDrive { get; set; }
        public string TotalWork { get; set; }
        public string TotalAvailability { get; set; }
        public double TotalDistance { get; set; }
        public List<ActivityDto> Activities { get; set; }
        public string TotalBreakRest { get; set; }
        public string TotalNightHour { get; set; }
        public string Id { get; set; }
        public string WeekNumber { get; set; }
    }
}
