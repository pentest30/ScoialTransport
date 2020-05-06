using System;

namespace tacchograaph_reader.Core.Entities
{
    public class Identifier : BaseEntity
    {

        public Guid? DriverId { get; set; }
        public string CardNumber { get; set; }
        public Driver Driver { get; set; }
        public DateTime CardIssueDate { get; set; }
        public DateTime CardValidityBegin { get; set; }
        public DateTime CardExpiryDate { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
