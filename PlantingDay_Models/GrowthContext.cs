


namespace PlantingDay.Models
{
    public class GrowthContext
    {
        // Time in the game
        //public int Today { get; set; }
        public SeasonId CurrentSeason { get; set; }
        public SeasonId NextSeason { get; set; }

        // Plant info
        //public List<Season> Seasons { get; set; } = new(); // list of seasons as enum for the plant
        public int ProduceDay { get; set; } // how many days until the first or only harvest 
        public int PaddyProduceDay { get; set; } // How many days wuntil first/only harvest if planted in a paddy


        public int ReadyDay { get; set; } // what day is the crop ready 
        public int PaddyReadyDay { get; set; } // ready day if planted in a paddy

        public int OverflowDay { get; set; } //ready day if multiseason crop is ready next season
        public int PaddyOverflowDay { get; set; } //overflow day if planted in a paddy

        public int? AdditionalSeasons { get; set; } // how many regrow seasons
        //public int RegrowDaysAvailable { get; set; } // how many days are available for regrowth
        public int RegrowQty { get; set; } // how many times will it regrow, including the first harvest
        public int PaddyRegrowQty { get; set; } // regrow quantity if placed in a paddy


    }
}
