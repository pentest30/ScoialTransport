using System;
using Microsoft.AspNetCore.Identity;

namespace tacchograaph_reader.Core.Entities
{
    public class UserApp : IdentityUser<Guid>
    {
        public Customer Customer { get; set; }
        public Guid? CustomerId { get; set; }
        public bool Active { get; set; }
    }
}
