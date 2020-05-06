using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace tacchograaph_reader.Core.Entities
{
    public sealed class Vehicle:BaseEntity
    {
        public Guid? Brand_Id { get; set; }
        public Guid? Model_Id { get; set; }

      

        public string VehicleName { get; set; }
        public string Vin { get; set; }
        public string LicensePlate { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<System.DateTime> InitServiceDate { get; set; }
        public VehicleType VehicleType { get; set; }
        public VehicleStatus VehicleStatus { get; set; }
        public Nullable<System.Guid> CustomerId { get; set; }
        public Customer Customer { get; set; }
        [ForeignKey("Brand_Id")]
        public Brand Brand { get; set; }
        [ForeignKey("Model_Id")]
        public Model Model { get; set; }
        public int MaxSpeed { get; set; }
        public int Milestone { get; set; }
        [NotMapped]
        public Guid? Box_Id { get; set; }
        public Guid? InteerestAreaId { get; set; }
        public ICollection<Driver> Drivers { get; set; }
    }
}
