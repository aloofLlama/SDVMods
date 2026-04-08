using PlantingDay.Helpers;
using PlantingDay.Models;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.ToolTip_Sections
{
    internal class PlantGrowthSection
    {
        public static List<TooltipElement> Build(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            if (plant.DaysToProduce <= 0)
                return list;

            // build growth context (e.g. when is it ready)
            GrowthContext growth = BuildGrowthContext(plant);

            //Various options for crops
            list.AddRange(CropsOutOfSeason(plant, growth));
            list.AddRange(CropsReadyThisSeason(plant, growth));
            list.AddRange(CropsReadyNextSeason(plant, growth)); // For multi-season crops
            list.AddRange(CropsTooLateToPlant(plant, growth));

            //Various options for paddy crops
            list.AddRange(PaddyCropsOutOfSeason(plant, growth));
            list.AddRange(PaddyCropsReadyThisSeason(plant, growth));
            list.AddRange(PaddyCropsReadyNextSeason(plant, growth)); // For multi-season crops
            list.AddRange(PaddyCropsTooLateToPlant(plant, growth));

            //Various options for fruit trees
            list.AddRange(FruitTreesReadyNextSeason(plant, growth)); //ready during in-season
            list.AddRange(FruitTreesReadyInFuture(plant, growth)); //ready on day 1 next in-season

            return list;

        }
        private static GrowthContext BuildGrowthContext(PlantInfo plant)
        {
            int today = Game1.dayOfMonth;

            SeasonId currentSeason = SeasonHelper.FromGameSeason(Game1.season);
            SeasonId nextSeason = SeasonHelper.Next(currentSeason);
            int additionalSeasons = SeasonHelper.CountAdditionalSeasons(currentSeason, plant.Seasons);

            int growDays = plant.DaysToProduce ?? 0;
            int paddyGrowDays = (int)Math.Floor(growDays / 1.25f);

            int readyDay = today + growDays;
            int paddyReadyDay = today + paddyGrowDays;

            int regrowDays = plant.RegrowDays.GetValueOrDefault();
            int paddyRegrowDays = (int)Math.Floor(regrowDays / 1.25f);

            int regrowDaysAvailable = 0;
            int paddyRegrowDaysAvailable = 0;

            int regrowQty;
            int paddyRegrowQty;


            // Regrow days for current season and additonal seasons for multi-harvest crops.
            if (plant.RegrowDays > 0 &&
                plant.Seasons.Contains(currentSeason) &&
                plant.PlantType == PlantType.Crop)
            {
                regrowDaysAvailable = ((1 + additionalSeasons) * 28) - readyDay;
                paddyRegrowDaysAvailable = ((1 + additionalSeasons) * 28) - paddyReadyDay;
            }

            // Regrow days for out of season
            if (plant.RegrowDays > 0 &&
                !plant.Seasons.Contains(currentSeason) &&
                plant.PlantType == PlantType.Crop)
            {
                regrowDaysAvailable = 28 * plant.Seasons.Count - growDays - 1;
                paddyRegrowDaysAvailable = 28 * plant.Seasons.Count - paddyGrowDays - 1;
            }

            // Regrow days for fruit trees next season
            if (plant.RegrowDays > 0 &&
                plant.Seasons.Contains(nextSeason) &&
                plant.PlantType == PlantType.FruitTree)
            {
                readyDay = today + growDays - 28 * additionalSeasons;
                regrowDaysAvailable = 28 - readyDay;
            }

            // Regrow days for fruit trees out of season / not ready in time
            if (plant.RegrowDays > 0 &&
                !plant.Seasons.Contains(nextSeason) &&
                plant.PlantType == PlantType.FruitTree)
            {
                readyDay = 1;
                regrowDaysAvailable = 27;
            }

            double r = (double)regrowDaysAvailable / regrowDays;
            regrowQty = 1 + (int)Math.Floor(r);

            double pr = (double)paddyRegrowDaysAvailable / paddyRegrowDays;
            paddyRegrowQty = 1 + (int)Math.Floor(pr);

            return new GrowthContext
            {
                CurrentSeason = currentSeason,
                NextSeason = nextSeason,

                PaddyProduceDay = paddyGrowDays,

                ReadyDay = readyDay,
                PaddyReadyDay = paddyReadyDay,

                OverflowDay = readyDay - 28,
                PaddyOverflowDay = paddyReadyDay - 28,

                RegrowQty = regrowQty,
                PaddyRegrowQty = paddyRegrowQty,
            };
        }

        //-----------------
        // Crops in/out of Season
        //-----------------

        // Crops | Out of Season
        private static List<TooltipElement> CropsOutOfSeason(PlantInfo plant, GrowthContext growth)
        {
            var list = new List<TooltipElement>();

            if (plant.PlantType != PlantType.Crop ||
                plant.Seasons.Contains(growth.CurrentSeason))
                return list;

            list.Add(new TooltipElement
            {
                IconRef = plant.HarvestIconRef,
                Text = string.Format(ModEntry.ModHelper.Translation
                    .Get(TooltipKeys.DaysToProduce),
                    plant.DaysToProduce
                ),
            });

            if (plant.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.RegrowQty),
                        growth.RegrowQty
                    ),
                });
            }

            return list;
        }

        // Crops | In season | Ready in time
        private static List<TooltipElement> CropsReadyThisSeason(PlantInfo plant, GrowthContext growth)
        {
            var list = new List<TooltipElement>();

            if (plant.PlantType != PlantType.Crop ||
                !plant.Seasons.Contains(growth.CurrentSeason) ||
                growth.ReadyDay > 28)
                return list;

            list.Add(new TooltipElement
            {
                IconRef = plant.HarvestIconRef,
                Text = string.Format(ModEntry.ModHelper.Translation
                    .Get(TooltipKeys.ReadyOn),
                    plant.DaysToProduce,
                    growth.CurrentSeason,
                    growth.ReadyDay
                ),
            });

            if (plant.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.RegrowQty),
                        growth.RegrowQty
                    ),
                });
            }
            return list;
        }

        // Crops | In Season | Ready Next Season
        private static List<TooltipElement> CropsReadyNextSeason(PlantInfo plant, GrowthContext growth)
        {

            var list = new List<TooltipElement>();

            if (plant.PlantType != PlantType.Crop ||
                !plant.Seasons.Contains(growth.CurrentSeason) ||
                !plant.Seasons.Contains(growth.NextSeason) ||
                growth.ReadyDay < 28)
                return list;

            list.Add(new TooltipElement
            {
                IconRef = plant.HarvestIconRef,
                Text = string.Format(ModEntry.ModHelper.Translation
                    .Get(TooltipKeys.ReadyOn),
                    plant.DaysToProduce,
                    growth.NextSeason,
                    growth.OverflowDay
                ),
            });

            if (plant.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.RegrowQty),
                        growth.RegrowQty
                    ),
                });
            }

            return list;
        }

        // Crops | In Season | Not Ready in Time
        private static List<TooltipElement> CropsTooLateToPlant(PlantInfo plant, GrowthContext growth)
        {
            var list = new List<TooltipElement>();

            if (plant.PlantType != PlantType.Crop ||
                !plant.Seasons.Contains(growth.CurrentSeason) ||
                plant.Seasons.Contains(growth.NextSeason) ||
                growth.ReadyDay < 28)
                return list;

            list.Add(new TooltipElement
            {
                IconRef = plant.HarvestIconRef,
                Text = string.Format(ModEntry.ModHelper.Translation
                    .Get(TooltipKeys.DaysToProduce),
                    plant.DaysToProduce
                ),
            });

            // Too late warning
            list.Add(new TooltipElement
            {
                IconRef = TooltipIcons.Warning,
                Text = string.Format(ModEntry.ModHelper.Translation
                    .Get(TooltipKeys.TooLate)),
                TextColor = TooltipColors.Warning
            });

            return list;
        }

        //-----------------
        // Paddy Crops in/out of Season
        //-----------------

        // Paddy | Crops | Out of Season
        private static List<TooltipElement> PaddyCropsOutOfSeason(PlantInfo plant, GrowthContext growth)
        {
            var list = new List<TooltipElement>();

            if (plant.PlantType != PlantType.Crop ||
                plant.Seasons.Contains(growth.CurrentSeason) ||
                !plant.Paddy)
                return list;

            list.Add(new TooltipElement
            {
                IconRef = TooltipIcons.WaterSeeds,
                Text = string.Format(ModEntry.ModHelper.Translation
                    .Get(TooltipKeys.DaysToProduce),
                    growth.PaddyProduceDay
                ),
                TextColor = TooltipColors.Paddy
            });

            if (plant.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.RegrowQty),
                        growth.PaddyRegrowQty
                    ),
                    TextColor = TooltipColors.Paddy
                });
            }

            return list;
        }

        // Paddy | Crops | In Season
        private static List<TooltipElement> PaddyCropsReadyThisSeason(PlantInfo plant, GrowthContext growth)
        {
            var list = new List<TooltipElement>();

            if (plant.PlantType != PlantType.Crop ||
                !plant.Seasons.Contains(growth.CurrentSeason) ||
                growth.PaddyReadyDay > 28 ||
                !plant.Paddy)
                return list;

            list.Add(new TooltipElement
            {
                IconRef = TooltipIcons.WaterSeeds,
                Text = string.Format(ModEntry.ModHelper.Translation
                    .Get(TooltipKeys.ReadyOn),
                    growth.PaddyProduceDay,
                    growth.CurrentSeason,
                    growth.PaddyReadyDay
                ),
                TextColor = TooltipColors.Paddy
            });

            if (plant.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.RegrowQty),
                        growth.PaddyRegrowQty
                    ),
                    TextColor = TooltipColors.Paddy
                });
            }
            return list;
        }

        // Paddy | Crops | Ready Next Season
        private static List<TooltipElement> PaddyCropsReadyNextSeason(PlantInfo plant, GrowthContext growth)
        {
            var list = new List<TooltipElement>();

            if (plant.PlantType != PlantType.Crop ||
                  !plant.Seasons.Contains(growth.CurrentSeason) ||
                  !plant.Seasons.Contains(growth.NextSeason) ||
                  growth.PaddyReadyDay < 28 ||
                  !plant.Paddy)
                return list;

            list.Add(new TooltipElement
            {
                IconRef = TooltipIcons.WaterSeeds,
                Text = string.Format(ModEntry.ModHelper.Translation
                    .Get(TooltipKeys.ReadyOn),
                    growth.PaddyProduceDay,
                    growth.NextSeason,
                    growth.PaddyOverflowDay),
                TextColor = TooltipColors.Paddy
            });

            if (plant.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.RegrowQty),
                        growth.PaddyRegrowQty),
                    TextColor = TooltipColors.Paddy
                });
            }
            return list;
        }

        // Paddy | Crops | In Season | Not Ready in Time
        private static List<TooltipElement> PaddyCropsTooLateToPlant(PlantInfo plant, GrowthContext growth)
        {
            var list = new List<TooltipElement>();

            if (plant.PlantType != PlantType.Crop ||
                  !plant.Seasons.Contains(growth.CurrentSeason) ||
                  plant.Seasons.Contains(growth.NextSeason) ||
                  growth.PaddyReadyDay < 28 ||
                  !plant.Paddy)
                return list;

            list.Add(new TooltipElement
            {
                IconRef = TooltipIcons.WaterSeeds,
                Text = string.Format(ModEntry.ModHelper.Translation
                    .Get(TooltipKeys.DaysToProduce),
                    growth.PaddyProduceDay
                ),
                TextColor = TooltipColors.Paddy
            });

            // Too late warning
            list.Add(new TooltipElement
            {
                IconRef = TooltipIcons.Warning,
                Text = string.Format(ModEntry.ModHelper.Translation
                    .Get(TooltipKeys.TooLate)
                ),
                TextColor = TooltipColors.Warning
            });
            return list;
        }

        //-----------------
        // Fruit Trees in/out of Season
        //-----------------

        // Fruit Trees | Ready Next Season
        private static List<TooltipElement> FruitTreesReadyNextSeason(PlantInfo plant, GrowthContext growth)
        {
            var list = new List<TooltipElement>();

            if (plant.PlantType != PlantType.FruitTree ||
                  !plant.Seasons.Contains(growth.NextSeason))
                return list;

            list.Add(new TooltipElement
            {
                IconRef = plant.HarvestIconRef,
                Text = string.Format(ModEntry.ModHelper.Translation
                    .Get(TooltipKeys.ReadyOn),
                    plant.DaysToProduce,
                    growth.NextSeason,
                    growth.ReadyDay
                ),
            });

            if (plant.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.RegrowQty),
                        growth.RegrowQty),
                });
            }
            return list;
        }

        // Fruit Trees | Ready After Next Season
        private static List<TooltipElement> FruitTreesReadyInFuture(PlantInfo plant, GrowthContext growth)
        {

            var list = new List<TooltipElement>();

            if (plant.PlantType != PlantType.FruitTree ||
                  plant.Seasons.Contains(growth.NextSeason))
                return list;

                list.Add(new TooltipElement
                {
                    IconRef = plant.HarvestIconRef,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.TreeReadyInFuture),
                        plant.Seasons?.FirstOrDefault()
                    ),
                });

                if (plant.RegrowDays > 0)
                {
                    list.Add(new TooltipElement
                    {
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.RegrowQty),
                            growth.RegrowQty
                        ),
                    });
            }
            return list;
        }

    }
}
