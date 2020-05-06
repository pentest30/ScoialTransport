using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TachographReader.Application.Dtos.Activities;
using TachographReader.Application.Queries;
using TachographReader.Application.Services;
using TachographReader.Web.Models;

namespace TachographReader.Web.Controllers
{
    public class SocialDataController : Controller
    {
        private readonly IDriverQueries driverService;
        private readonly IDriverCarReportService driverCarReport;

        public SocialDataController(IDriverQueries driverService, IDriverCarReportService driverCarReport)
        {
            this.driverService = driverService;
            this.driverCarReport = driverCarReport;
        }
        public  async Task<IActionResult> Index()
        {
            var customerId = Guid.Parse("13bce473-5e31-4f3f-87a0-8863b6a814f5");
            ViewData["drivers"] = new SelectList( await  driverService.GetDriversAsync(customerId).ConfigureAwait(false), "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetSocialDataReport(Guid driverId, DateTime startPeriod, DateTime endPeriod, string type)
        {
            var report = await driverCarReport.GetDriverCardReportFromByPeriodAsync(driverId, startPeriod, endPeriod)
                .ConfigureAwait(false);
            var total = GetTotalService(report);
            return Json(new {totalService = total, dailyActivities = report.DailyActivities.Select(x=> new DriverDailyActivityVViewModel(x))});
        }

        private static dynamic GetTotalService(DriverPeriodActivitiesDto report)
        {
            var totalService = new
            {
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