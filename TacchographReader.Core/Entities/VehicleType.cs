using System;
using System.ComponentModel.DataAnnotations;

namespace tacchograaph_reader.Core.Entities
{
    public enum VehicleType:Int32
    {
        [Display(Name = "Véhicule léger")]
        Car,
        [Display(Name = "Tracteur")]
        Track,
        [Display(Name = "Bus")]
        Bus

    }
}