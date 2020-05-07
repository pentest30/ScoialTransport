using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TachographReader.Application.Dtos.Driver;
using TachographReader.Application.Queries;
using TachographReader.Web.Models;

namespace TachographReader.Web.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly IDriverQueries driverQueries;
        private readonly IMediator mediator;

        public DriversController(IDriverQueries driverQueries,IMediator mediator)
        {
            this.driverQueries = driverQueries;
            this.mediator = mediator;
        }

        

        [HttpGet("{customerId}", Name = "GetAll")]
        public Task<IEnumerable<DriverDto>> GetAll(string customerId)
        {
            return driverQueries.GetListOfDriversForApiAsync(Guid.Parse( customerId));
        }

        // POST: api/Driver

        public Task<IActionResult> Edit(AddOrUpdateDriveViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
