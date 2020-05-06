﻿using System;
using System.Collections.Generic;

namespace tacchograaph_reader.Core.Entities
{
    public class Model : BaseEntity
    {
        public string Name { get; set; }
        public Nullable<System.Guid> Brand_Id { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
