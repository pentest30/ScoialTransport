using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace tacchograaph_reader.Core.Entities
{
    public class Customer:BaseEntity
    {
        public Customer()
        {
            Users = new List<IdentityUser>();
            Vehicles = new List<Vehicle>();
            Drivers = new List<Driver>();
        }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public System.DateTime? CreationDate { get; set; }
        public Address Address { get; set; }
        public CustomerStatus CustomerStatus { get; set; }
        public  ICollection<Driver> Drivers { get; set; }
        public  ICollection<IdentityUser> Users { get; set; }
        public  ICollection<Vehicle> Vehicles { get; set; }
        public string LocalCulture { get; set; }
    }

    public enum CustomerStatus
    {
        Active,
        Suspended,
        Deleted
    }
}
