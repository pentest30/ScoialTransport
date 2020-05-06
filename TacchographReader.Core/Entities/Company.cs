
namespace tacchograaph_reader.Core.Entities
{
    public class Company :BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public Address Address { get; set; }

    }
}
