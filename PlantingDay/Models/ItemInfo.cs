using PlantingDay.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace PlantingDay.Models
{
    public class ItemInfo
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = ""; // e.g. Crafting, Fish, Cooking, Seeds, Ring 

        public int Price { get; set; }
        public int Category { get; set; } //e.g. seeds, vegetable,fruit
        public int Edibility { get; set; }

        //[JsonIgnore]
        //public IconRef? Icon { get; set; }

    }

}
