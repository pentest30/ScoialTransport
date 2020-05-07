using System.Threading;
using System.Threading.Tasks;
using MediatR;
using tacchograaph_reader.Core.Commands.Driver;
using TachographReader.Application.helpers;
using TachoReader.Data.Data;

namespace TachographReader.Application.Handlers
{
    public class Driverhandler:AsyncRequestHandler<EditDriverDto>
    {
        protected override  async Task Handle(EditDriverDto request, CancellationToken cancellationToken)
        {
            var context = new ApplicationDbContext(ConfigHelper.DbContextOptionsBuilder.Options);
            var existingDriver = await context.Drivers.FindAsync(request.Id).ConfigureAwait(false);
            if (existingDriver != null)
            {
                existingDriver.Name = request.FullName;
                existingDriver.BirthDate = request.BirthDate;
                existingDriver.DriverNumber = request.DrivingLicenseNumber;
                existingDriver.Tel = request.Phone;
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
