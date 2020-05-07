using System;
using MediatR;

namespace tacchograaph_reader.Core.Commands.Driver
{
    public class EditDriverDto :IRequest
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string DrivingLicenseNumber { get; set; }
        public string Phone { get; set; }
    }
}
