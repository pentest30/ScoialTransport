using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TachographReader.Application.Dtos.Driver;
using TachographReader.Application.Queries;

namespace TachographReader.Web.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriverQueries driverQueries;

        public DriverController(IDriverQueries driverQueries)
        {
            this.driverQueries = driverQueries;
        }

        // GET: api/Driver
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        [HttpGet("{customerId}", Name = "GetAll")]
        public Task<IEnumerable<DriverDto>> GetAll(string customerId)
        {
            return driverQueries.GetListOfDriversForApiAsync(Guid.Parse( customerId));
        }

        // POST: api/Driver
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Driver/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
