using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVData;

namespace PlantingDay.Helpers
{
    public static class SeasonHelper
    {
        public static SeasonId FromGameSeason(StardewValley.Season s)
        {
            return s switch
            {
                StardewValley.Season.Spring => SeasonId.Spring,
                StardewValley.Season.Summer => SeasonId.Summer,
                StardewValley.Season.Fall => SeasonId.Fall,
                StardewValley.Season.Winter => SeasonId.Winter,
                _ => SeasonId.Spring
            };
        }

        public static string Translate(SeasonId season)
        {
            string key = season switch
            {
                SeasonId.Spring => "season.spring",
                SeasonId.Summer => "season.summer",
                SeasonId.Fall => "season.fall",
                SeasonId.Winter => "season.winter",
                _ => "season.unknown"
            };

            return ModEntry.ModHelper.Translation.Get(key);
        }


        public static int CountAdditionalSeasons(SeasonId current, List<SeasonId> allowed)
        {
            if (allowed == null || allowed.Count == 0)
                return 0;

            int count = 0;
            SeasonId s = Next(current);

            // Maximum of 3 additional seasons in a year
            for (int i = 0; i < 3; i++)
            {
                if (!allowed.Contains(s))
                    break;

                count++;
                s = Next(s);
            }

            return count;
        }

        public static SeasonId Next(SeasonId s)
        {
            return s switch
            {
                SeasonId.Spring => SeasonId.Summer,
                SeasonId.Summer => SeasonId.Fall,
                SeasonId.Fall => SeasonId.Winter,
                SeasonId.Winter => SeasonId.Spring,
                _ => SeasonId.Spring
            };
        }




    }
}
