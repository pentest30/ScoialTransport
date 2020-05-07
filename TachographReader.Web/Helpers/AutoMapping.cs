using AutoMapper;
using tacchograaph_reader.Core.Commands.Driver;
using TachographReader.Web.Models;

namespace TachographReader.Web.Helpers
{
    public class AutoMapping: Profile
    {
        public AutoMapping()
        {
            CreateMap<EditDriverDto, AddOrUpdateDriveViewModel>().ReverseMap();
        }
    }
}
