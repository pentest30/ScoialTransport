using System;
using System.Threading.Tasks;
using TachographReader.Application.Dtos.Activities;

namespace TachographReader.Application.Services
{
    public interface IDriverCarReportService
    {
        Task<DriverPeriodActivitiesDto> GetDriverCardReportFromByPeriodAsync(Guid driverId, DateTime start, DateTime end);
    }
}