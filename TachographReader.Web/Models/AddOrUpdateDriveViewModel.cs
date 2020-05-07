using System;
using tacchograaph_reader.Core.Entities;

namespace TachographReader.Web.Models
{
    public class AddOrUpdateDriveViewModel
    {
        public AddOrUpdateDriveViewModel()
        {
            
        }
        public AddOrUpdateDriveViewModel(Driver driver)
        {
            Id = driver.Id;
            FullName = driver.Name;
            BirthDate = driver.BirthDate;
            Phone = driver.Tel;
            DrivingLicenseNumber = driver.DriverNumber;
        }
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string DrivingLicenseNumber { get; set; }
        public string Phone { get; set; }
    }
}