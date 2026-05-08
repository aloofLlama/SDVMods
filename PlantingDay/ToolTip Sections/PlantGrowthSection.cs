
using PlantingDay.Helpers;
using PlantingDay.Models.Runtime;
using PlantingDay.Models.Wrappers;
using SDVCommon;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVCommon.Tooltip;
using SDVData;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlantingDay.ToolTip_Sections
{
    internal class PlantGrowthSection
    {
        public static List<TooltipElement> Build(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            if (plant.Data.DaysToProduce <= 0)
                return list;

            var harvestIcon =
                HarvestInfoBuilder.LookupFromKey(plant.Data.HarvestId)?.Runtime.HarvestIcon;

            GrowthContext growth = BuildGrowthContext(plant);

            list.AddRange(CropsYearround(plant, growth, harvestIcon));
            list.AddRange(CropsOutOfSeason(plant, growth, harvestIcon));
            list.AddRange(CropsReadyThisSeason(plant, growth, harvestIcon));
            list.AddRange(CropsReadyNextSeason(plant, growth, harvestIcon));
            list.AddRange(CropsTooLateToPlant(plant, growth, harvestIcon));

            list.AddRange(PaddyCropsOutOfSeason(plant, growth, harvestIcon));
            list.AddRange(PaddyCropsReadyThisSeason(plant, growth, harvestIcon));
            list.AddRange(PaddyCropsReadyNextSeason(plant, growth, harvestIcon));
            list.AddRange(PaddyCropsTooLateToPlant(plant, growth, harvestIcon));

            list.AddRange(FruitTreesReadyNextSeason(plant, growth, harvestIcon));
            list.AddRange(FruitTreesReadyInFuture(plant, growth, harvestIcon));

            return list;
        }

        private static GrowthContext BuildGrowthContext(PlantInfo plant)
        {
            int today = Game1.dayOfMonth;

            SeasonId currentSeason = SeasonHelper.FromGameSeason(Game1.season);
            SeasonId nextSeason = SeasonHelper.Next(currentSeason);
            int additionalSeasons = SeasonHelper.CountAdditionalSeasons(currentSeason, plant.Data.Seasons);

            int growDays = plant.Data.DaysToProduce ?? 0;
            int paddyGrowDays = (int)Math.Floor(growDays / 1.25f);

            int readyDay = today + growDays;
            int paddyReadyDay = today + paddyGrowDays;

            int regrowDays = plant.Data.RegrowDays.GetValueOrDefault();
            int paddyRegrowDays = (int)Math.Floor(regrowDays / 1.25f);

            int regrowDaysAvailable = 0;
            int paddyRegrowDaysAvailable = 0;

            int regrowQty;
            int paddyRegrowQty;

            if (plant.Data.RegrowDays > 0 &&
                plant.Data.Seasons.Contains(currentSeason) &&
                plant.Data.PlantType == PlantType.Crop)
            {
                regrowDaysAvailable = ((1 + additionalSeasons) * 28) - readyDay;
                paddyRegrowDaysAvailable = ((1 + additionalSeasons) * 28) - paddyReadyDay;
            }

            if (plant.Data.RegrowDays > 0 &&
                !plant.Data.Seasons.Contains(currentSeason) &&
                plant.Data.PlantType == PlantType.Crop)
            {
                regrowDaysAvailable = 28 * plant.Data.Seasons.Count - growDays - 1;
                paddyRegrowDaysAvailable = 28 * plant.Data.Seasons.Count - paddyGrowDays - 1;
            }

            if (plant.Data.RegrowDays > 0 &&
                plant.Data.Seasons.Contains(nextSeason) &&
                plant.Data.PlantType == PlantType.FruitTree)
            {
                readyDay = today + growDays - 28 * additionalSeasons;
                regrowDaysAvailable = 28 - readyDay;
            }

            if (plant.Data.RegrowDays > 0 &&
                !plant.Data.Seasons.Contains(nextSeason) &&
                plant.Data.PlantType == PlantType.FruitTree)
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

        // -------------------------
        // Crops
        // -------------------------

        private static List<TooltipElement> CropsYearround(
            PlantInfo plant, GrowthContext growth, Icon? harvestIcon)
        {
            var list = new List<TooltipElement>();

            if (plant.Data.PlantType != PlantType.Crop ||
                plant.Data.Seasons.Count != 4)
                return list;

            list.Add(new TooltipElement
            {
                Icon = harvestIcon,
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.DaysToProduce),
                    plant.Data.DaysToProduce)
            });

            if (plant.Data.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Get(IconKey.Spiral),
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.RegrowIndef))
                    
                });
            }

            return list;
        }

        private static List<TooltipElement> CropsOutOfSeason(
            PlantInfo plant, GrowthContext growth, Icon? harvestIcon)
        {
            var list = new List<TooltipElement>();

            if (plant.Data.PlantType != PlantType.Crop ||
                plant.Data.Seasons.Contains(growth.CurrentSeason))
                return list;

            list.Add(new TooltipElement
            {
                Icon = harvestIcon,
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.DaysToProduce),
                    plant.Data.DaysToProduce)
            });

            if (plant.Data.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Get(IconKey.Spiral),
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.RegrowQty),
                        growth.RegrowQty)
                });
            }

            return list;
        }

        private static List<TooltipElement> CropsReadyThisSeason(
            PlantInfo plant, GrowthContext growth, Icon? harvestIcon)
        {
            var list = new List<TooltipElement>();

            if (plant.Data.PlantType != PlantType.Crop ||
                !plant.Data.Seasons.Contains(growth.CurrentSeason) ||
                plant.Data.Seasons.Count == 4 ||
                growth.ReadyDay > 28)
                return list;

            list.Add(new TooltipElement
            {
                Icon = harvestIcon,
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.ReadyOn),
                    plant.Data.DaysToProduce,
                    SeasonHelper.Translate(growth.CurrentSeason),
                    growth.ReadyDay)
            });

            if (plant.Data.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Get(IconKey.Spiral),
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.RegrowQty),
                        growth.RegrowQty)
                });
            }

            return list;
        }

        private static List<TooltipElement> CropsReadyNextSeason(
            PlantInfo plant, GrowthContext growth, Icon? harvestIcon)
        {
            var list = new List<TooltipElement>();

            if (plant.Data.PlantType != PlantType.Crop ||
                !plant.Data.Seasons.Contains(growth.CurrentSeason) ||
                !plant.Data.Seasons.Contains(growth.NextSeason) ||
                plant.Data.Seasons.Count == 4 ||
                growth.ReadyDay <= 28)
                return list;

            list.Add(new TooltipElement
            {
                Icon = harvestIcon,
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.ReadyOn),
                    plant.Data.DaysToProduce,
                    SeasonHelper.Translate(growth.NextSeason),
                    growth.OverflowDay)
            });

            if (plant.Data.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Get(IconKey.Spiral),
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.RegrowQty),
                        growth.RegrowQty)
                });
            }

            return list;
        }

        private static List<TooltipElement> CropsTooLateToPlant(
            PlantInfo plant, GrowthContext growth, Icon? harvestIcon)
        {
            var list = new List<TooltipElement>();

            if (plant.Data.PlantType != PlantType.Crop ||
                !plant.Data.Seasons.Contains(growth.CurrentSeason) ||
                plant.Data.Seasons.Contains(growth.NextSeason) ||
                growth.ReadyDay <= 28)
                return list;

            list.Add(new TooltipElement
            {
                Icon = harvestIcon,
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.DaysToProduce),
                    plant.Data.DaysToProduce)
            });

            list.Add(new TooltipElement
            {
                Icon = TooltipIcons.Get(IconKey.Warning),
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.TooLate)),
                TextColor = TooltipColors.Warning
            });

            return list;
        }

        // -------------------------
        // Paddy Crops
        // -------------------------

        private static List<TooltipElement> PaddyCropsOutOfSeason(
            PlantInfo plant, GrowthContext growth, Icon? harvestIcon)
        {
            var list = new List<TooltipElement>();

            if (plant.Data.PlantType != PlantType.Crop ||
                plant.Data.Seasons.Contains(growth.CurrentSeason) ||
                !plant.Data.Paddy)
                return list;

            list.Add(new TooltipElement
            {
                Icon = TooltipIcons.Get(IconKey.WaterSeeds),
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.DaysToProduce),
                    growth.PaddyProduceDay),
                TextColor = TooltipColors.Paddy
            });

            if (plant.Data.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Get(IconKey.Spiral),
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.RegrowQty),
                        growth.PaddyRegrowQty),
                    TextColor = TooltipColors.Paddy
                });
            }

            return list;
        }

        private static List<TooltipElement> PaddyCropsReadyThisSeason(
            PlantInfo plant, GrowthContext growth, Icon? harvestIcon)
        {
            var list = new List<TooltipElement>();

            if (plant.Data.PlantType != PlantType.Crop ||
                !plant.Data.Seasons.Contains(growth.CurrentSeason) ||
                growth.PaddyReadyDay > 28 ||
                !plant.Data.Paddy)
                return list;

            list.Add(new TooltipElement
            {
                Icon = TooltipIcons.Get(IconKey.WaterSeeds),
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.ReadyOn),
                    growth.PaddyProduceDay,
                    SeasonHelper.Translate(growth.CurrentSeason),
                    growth.PaddyReadyDay),
                TextColor = TooltipColors.Paddy
            });

            if (plant.Data.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Get(IconKey.Spiral),
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.RegrowQty),
                        growth.PaddyRegrowQty),
                    TextColor = TooltipColors.Paddy
                });
            }

            return list;
        }

        private static List<TooltipElement> PaddyCropsReadyNextSeason(
            PlantInfo plant, GrowthContext growth, Icon? harvestIcon)
        {
            var list = new List<TooltipElement>();

            if (plant.Data.PlantType != PlantType.Crop ||
                !plant.Data.Seasons.Contains(growth.CurrentSeason) ||
                !plant.Data.Seasons.Contains(growth.NextSeason) ||
                growth.PaddyReadyDay <= 28 ||
                !plant.Data.Paddy)
                return list;

            list.Add(new TooltipElement
            {
                Icon = TooltipIcons.Get(IconKey.WaterSeeds),
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.ReadyOn),
                    growth.PaddyProduceDay,
                    SeasonHelper.Translate(growth.NextSeason),
                    growth.PaddyOverflowDay),
                TextColor = TooltipColors.Paddy
            });

            if (plant.Data.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Get(IconKey.Spiral),
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.RegrowQty),
                        growth.PaddyRegrowQty),
                    TextColor = TooltipColors.Paddy
                });
            }

            return list;
        }

        private static List<TooltipElement> PaddyCropsTooLateToPlant(
            PlantInfo plant, GrowthContext growth, Icon? harvestIcon)
        {
            var list = new List<TooltipElement>();

            if (plant.Data.PlantType != PlantType.Crop ||
                !plant.Data.Seasons.Contains(growth.CurrentSeason) ||
                plant.Data.Seasons.Contains(growth.NextSeason) ||
                growth.PaddyReadyDay <= 28 ||
                !plant.Data.Paddy)
                return list;

            list.Add(new TooltipElement
            {
                Icon = TooltipIcons.Get(IconKey.WaterSeeds),
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.DaysToProduce),
                    growth.PaddyProduceDay),
                TextColor = TooltipColors.Paddy
            });

            list.Add(new TooltipElement
            {
                Icon = TooltipIcons.Get(IconKey.Warning),
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.TooLate)),
                TextColor = TooltipColors.Warning
            });

            return list;
        }

        // -------------------------
        // Fruit Trees
        // -------------------------

        private static List<TooltipElement> FruitTreesReadyNextSeason(
            PlantInfo plant, GrowthContext growth, Icon? harvestIcon)
        {
            var list = new List<TooltipElement>();

            if (plant.Data.PlantType != PlantType.FruitTree ||
                !plant.Data.Seasons.Contains(growth.NextSeason))
                return list;

            list.Add(new TooltipElement
            {
                Icon = harvestIcon,
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.ReadyOn),
                    plant.Data.DaysToProduce,
                    SeasonHelper.Translate(growth.NextSeason),
                    growth.ReadyDay)
            });

            if (plant.Data.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Get(IconKey.Spiral),
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.RegrowQty),
                        growth.RegrowQty)
                });
            }

            return list;
        }

        private static List<TooltipElement> FruitTreesReadyInFuture(
            PlantInfo plant, GrowthContext growth, Icon? harvestIcon)
        {
            var list = new List<TooltipElement>();

            if (plant.Data.PlantType != PlantType.FruitTree ||
                plant.Data.Seasons.Contains(growth.NextSeason))
                return list;

            list.Add(new TooltipElement
            {
                Icon = harvestIcon,
                Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.TreeReadyInFuture),
                    SeasonHelper.Translate(plant.Data.Seasons.First()))
            });

            if (plant.Data.RegrowDays > 0)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Get(IconKey.Spiral),
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.RegrowQty),
                        growth.RegrowQty)
                });
            }

            return list;
        }
    }
}



