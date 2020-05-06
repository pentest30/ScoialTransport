using System;
using DataTables.AspNetCore.Mvc.Binder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace TachographReader.Web.Controllers
{
    public class DriverController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public DriverController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetList([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var customerId = Guid.Parse("13bce473-5e31-4f3f-87a0-8863b6a814f5");

            throw new System.NotImplementedException();
        }
    }
}