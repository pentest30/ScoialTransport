using System;

namespace TachographReader.Web.Models
{
    public class DriverQueryViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DrivingLicenseNumber { get; set; }
        public string Phone { get; set; }
        public string CardNumber { get; set; }
        public Guid Id { get; set; }
    }
}
