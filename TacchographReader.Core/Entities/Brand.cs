using System.Collections.Generic;

namespace tacchograaph_reader.Core.Entities
{
    public class Brand:BaseEntity
    {
        public string Name { get; set; }
        public  ICollection<Model> Models { get; set; }
        public  ICollection<Vehicle> Vehicles { get; set; }

    }
}
