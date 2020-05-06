using Microsoft.EntityFrameworkCore;

namespace tacchograaph_reader.Core.Entities
{
    [Owned]
    public class Address
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string ZipCode { get; set; }
    }
}
