using System;
using System.Threading.Tasks;
using DataTables.AspNetCore.Mvc.Binder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TachographReader.Application.Queries;

namespace TachographReader.Web.Controllers
{
    public class DriverController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IDriverQueries driverQueries;

        public DriverController(IHostingEnvironment hostingEnvironment, IDriverQueries driverQueries)
        {
            _hostingEnvironment = hostingEnvironment;
            this.driverQueries = driverQueries;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetList([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var customerId = Guid.Parse("13bce473-5e31-4f3f-87a0-8863b6a814f5");
            var result = await driverQueries.GetLisOfDriversAsync(customerId, dataRequest.Search?.Value, dataRequest.Start,
                dataRequest.Length).ConfigureAwait(false);
            return Json(result.Data.ToDataTablesResponse(dataRequest, result.RecordsTotal, result.RecordsFilterd));

        }
    }
}