using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml;
using TachographReader.Application.Queries;
using TachographReader.Application.Services;
using TachographReader.Web.Models;

namespace TachographReader.Web.Controllers
{
    public class SocialDataController : Controller
    {
        private readonly IDriverQueries driverService;
        private readonly IDriverCarReportService driverCarReport;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IMemoryCache memoryCache;

        public SocialDataController(IDriverQueries driverService, 
            IDriverCarReportService driverCarReport, IHostingEnvironment hostingEnvironment, IMemoryCache memoryCache)
        {
            this.driverService = driverService;
            this.driverCarReport = driverCarReport;
            this.hostingEnvironment = hostingEnvironment;
            this.memoryCache = memoryCache;
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
            var total = SummaryTotalService.GetTotalService(report);
            var key1 = HttpContext.Connection.Id + "dailyServices";
            var key2 = HttpContext.Connection.Id + "summaryServices";
            var dailyServices = report.DailyActivities.Select(x => new DriverDailyActivityVViewModel(x));
            if (memoryCache.Get(key1)!= null)
                memoryCache.Remove(key1);
            if (memoryCache.Get(key2) != null)
                memoryCache.Remove(key2);

            var driverDailyActivityVViewModels = dailyServices as DriverDailyActivityVViewModel[] ?? dailyServices.ToArray();
            memoryCache.Set(key1, driverDailyActivityVViewModels);
            memoryCache.Set(key2, total);
            return Json(new {totalService = total, dailyActivities = driverDailyActivityVViewModels});
        }

        [HttpGet]
        public FileResult ExportExcel()
        {
            string webRootPath = hostingEnvironment.WebRootPath;
            FileStream fs = new FileStream(string.Concat(webRootPath,
                        Path.DirectorySeparatorChar + "reports" + Path.DirectorySeparatorChar + "TachyReportDriver.xlsx"),
                    FileMode.Open);
            var stream = new MemoryStream();
            var key = HttpContext.Connection.Id + "dailyServices";
            var key2 = HttpContext.Connection.Id + "summaryServices";
            var report = memoryCache.Get<DriverDailyActivityVViewModel[]>(key) ;
            var summaryTotalService = memoryCache.Get<SummaryTotalService>(key2) ;
            using (ExcelPackage package = new ExcelPackage(stream, fs))
            {
                ExcelWorksheet sl = package.Workbook.Worksheets["Hours"];
                sl.Cells[2, 1].Value = "Detailed report from " + summaryTotalService?.StartPeriod.ToLocalTime().ToString("dd/MM/yyyy") +
                                       " to " + summaryTotalService?.EndPeriod.ToLocalTime().ToString("dd/MM/yyyy") + " of " +
                                       summaryTotalService.DriverName;
                sl.Cells[5, 2].Value = summaryTotalService.TotalService;
                sl.Cells[6, 2].Value = summaryTotalService.TotalAvailability;
                sl.Cells[7, 2].Value = summaryTotalService.TotalWork;
                sl.Cells[8, 2].Value = summaryTotalService.TotalDrive;
                sl.Cells[9, 2].Value = summaryTotalService.TotalNightHour;
                for (int i = 0; i < report?.Length; i++)
                {
                    sl.Cells[12 + i, 1].Value = report[i].Date.ToShortDateString();
                    sl.Cells[12 + i, 2].Value = report[i].TotalService;
                    sl.Cells[12 + i, 3].Value = report[i].TotalDrive;
                    sl.Cells[12 + i, 4].Value = report[i].TotalWork;
                    sl.Cells[12 + i, 5].Value = report[i].TotalAvailability;
                    sl.Cells[12 + i, 6].Value = report[i].TotalNightHour;
                }

                // save our new workbook and we are done!
                package.Save();
                stream.Position = 0;
                string excelName = $"tacho-driver-report-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

                //return File(stream, "application/octet-stream", excelName);  
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

     }
}