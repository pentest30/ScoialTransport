using System;


namespace tacchograaph_reader.Core.Entities
{
    public class Driver : BaseEntity
    {
        public string Name { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public DateTime? OnService { get; set; }
        public string DriverNumber{ get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
