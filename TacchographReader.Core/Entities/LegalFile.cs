using System;

namespace tacchograaph_reader.Core.Entities
{
    public class LegalFile : BaseEntity
    {
        public Guid? VehicleId { get; set; }
        public Guid? DriverId { get; set; }
        public DateTime GenerationDateUtc { get; set; }
        public Byte[] FileContent { get; set; }
        public String FileName { get; set; }
        public Guid CustomerId { get; set; }
        public Vehicle Vehicle { get; set; }
        public Driver Driver { get; set; }
        public Customer Customer { get; set; }

    }
}
