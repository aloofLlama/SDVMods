

namespace SDVData
{
    public class GrowthContext
    {
        // Time in the game
        public SeasonId CurrentSeason { get; set; }
        public SeasonId NextSeason { get; set; }

        // Plant info
        //Produce day already comes from PlantInfo from the game data.
        public int PaddyProduceDay { get; set; } // How many days wuntil first/only harvest if planted in a paddy

        public int ReadyDay { get; set; } // what day is the crop ready 
        public int PaddyReadyDay { get; set; } // ready day if planted in a paddy

        public int OverflowDay { get; set; } //ready day if multiseason crop is ready next season
        public int PaddyOverflowDay { get; set; } //overflow day if planted in a paddy

        public int RegrowQty { get; set; } // how many times will it regrow, including the first harvest
        public int PaddyRegrowQty { get; set; } // regrow quantity if placed in a paddy

    }
}
