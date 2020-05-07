using System;
using System.Threading.Tasks;
using AutoMapper;
using DataTables.AspNetCore.Mvc.Binder;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using tacchograaph_reader.Core.Commands.Driver;
using TachographReader.Application.Queries;
using TachographReader.Web.Models;

namespace TachographReader.Web.Controllers
{
    public class DriverController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IDriverQueries driverQueries;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public DriverController(IHostingEnvironment hostingEnvironment
            , IDriverQueries driverQueries
            ,IMapper mapper,IMediator mediator)
        {
            _hostingEnvironment = hostingEnvironment;
            this.driverQueries = driverQueries;
            this.mapper = mapper;
            this.mediator = mediator;
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
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model  = new AddOrUpdateDriveViewModel( await driverQueries.GetDriverByIdAsync(id).ConfigureAwait(false));
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditDriver(AddOrUpdateDriveViewModel model)
        {
            var driverDto = mapper.Map<EditDriverDto>(model);
            await mediator.Send(driverDto).ConfigureAwait(false);
            return View("Index");
        }
    }
}