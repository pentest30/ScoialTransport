using TachoReader.Data.Data;

namespace TachographReader.Application.Services
{
    public class BaseService
    {
        public  ApplicationDbContext Context { get; private set; }
        public BaseService(ApplicationDbContext context)
        {
            Context = context;
        }
    }
}
