using System;

namespace TachographReader.Application.Dtos
{
    public class DriverQueryViewModel
    {
        public string FullName { get; set; }
       
        public string DrivingLicenseNumber { get; set; }
        public string Phone { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string LastDownloadDate { get; set; }
        public Guid Id { get; set; }
    }
}
