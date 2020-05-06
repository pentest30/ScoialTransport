using MediatR;
using Microsoft.AspNetCore.Mvc;
using TachoReader.Data.Data;

namespace TachographReader.Web.Controllers
{
    public class BaseController : Controller
    {
        public readonly IMediator Mediator;
        public readonly ApplicationDbContext Context;

        public BaseController(IMediator mediator , ApplicationDbContext context)
        {
            Mediator = mediator;
            Context = context;
        }
    }
}