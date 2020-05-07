using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TachographReader.Application.Dtos.Driver
{
    public class DriverDto
    {
        public DriverDto()
        {
            Identifiers = new List<IdentifierDto>();
        }
        [JsonProperty(PropertyName="fullName")]
        public string FullName { get; set; }
        [JsonProperty(PropertyName = "drivingLicenseNumber")]
        public string DrivingLicenseNumber { get; set; }
        [JsonProperty(PropertyName = "phoneNumber")]
        public string Phone { get; set; }
        [JsonProperty(PropertyName = "lastDownloadDate")]
        public DateTime LastDownloadDate { get; set; }
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "identifiers")]
        public IEnumerable<IdentifierDto> Identifiers { get; set; }
        [JsonProperty(PropertyName = "birthDate")]
        public DateTime BirthDate { get; set; }
    }
}
