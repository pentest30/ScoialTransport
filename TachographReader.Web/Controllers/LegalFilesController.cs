using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTables.AspNetCore.Mvc.Binder;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tacchograaph_reader.Core.Commands.DddFiles;
using TachographReader.Application.Queries;
using TachoReader.Data.Data;

namespace TachographReader.Web.Controllers
{
    public class LegalFilesController : BaseController
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly IDriverQueries driverService;

        public LegalFilesController(IHostingEnvironment hostingEnvironment, 
            ApplicationDbContext context, 
            IMediator mediator,
            IDriverQueries driverService) : base(mediator, context)
        {
            _hostingEnvironment = hostingEnvironment;
            this.driverService = driverService;
        }
        public IActionResult Index()
        {
            ViewData["activetree"] = "active";
            return View();
        }

        public async Task<IActionResult> Uploader(IList<IFormFile> files)
        {
            try
            {
                await Mediator.Send(new AddDDDFileCommand
                {
                    DddFiles = files,
                    HostingEnvironment = _hostingEnvironment,
                    Paths = new List<string>()
                }).ConfigureAwait(false);
                return Json("File was successfully saved");
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        public async Task<IActionResult> GetList([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var customerId = Guid.Parse("13bce473-5e31-4f3f-87a0-8863b6a814f5");
            var result =await driverService.GetLegalFilesAsync(customerId, dataRequest.Search?.Value, dataRequest.Start,
                dataRequest.Length).ConfigureAwait(false);
            return Json(result.Data.ToDataTablesResponse(dataRequest, result.RecordsTotal, result.RecordsFilterd));

        }
    }
}