using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay;
using PlantingDay.Helpers;
using PlantingDay.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Crops;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;



namespace PlantingDay
{
    public static class TooltipBuilder
    {
        public static List<TooltipElement> BuildTooltip(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            list.AddRange(GetSeasonsTooltip(plant));

            // Growth info (includes DaysToProduce + ready day + warnings + Multiharvest)
            list.AddRange(GetPlantGrowthTooltip(plant));


            if (plant.Trellis && plant.PlantType == PlantType.Crop)
            {
                list.Add(new TooltipElement
                {
                    IconTexture = plant.SeedIconTexture,
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.RequiresTrellis)),
                    TextColor = TooltipColors.Normal
                });
            }

            //No watering
            if (!plant.NeedsWatering && plant.PlantType == PlantType.Crop)
            {
                list.Add(new TooltipElement
                {
                    IconRef = TooltipIcons.Watercan,
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.NoWatering)),
                    TextColor = TooltipColors.Normal
                });
            }


            //Harvest with scythe
            if (plant.Scythe == HarvestMethod.Scythe && plant.PlantType == PlantType.Crop)
            {
                list.Add(new TooltipElement
                {
                    IconRef = TooltipIcons.Scythe,
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.HarvestWithScythe)),
                    TextColor = TooltipColors.Normal
                });
            }


            //Multicolor sprites
            if (plant.MultiSprite > 0)
            {
                list.Add(new TooltipElement
                {
                    IconRef = TooltipIcons.Rainbow,
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.MultiSprite),
                        plant.MultiSprite),
                    TextColor = TooltipColors.Normal
                });
            }

            list.Add(new TooltipElement { IsSeparator = true, PaddingTop = 6, PaddingBottom = 6 });

            list.AddRange(GetEconomicsTooltip(plant));

            return list;
        }

        //--------------
        // Season display
        //--------------
        private static List<TooltipElement> GetSeasonsTooltip(PlantInfo plant)
        {
            var list = new List<TooltipElement>();
            if (plant.Seasons == null || plant.Seasons.Count == 0)
                return list;

            // Display the relevant seasons and highlight the current season
            if (plant.Seasons.Count > 0)
            {
                var segments = TooltipRenderer.BuildInlineSegments(
                    plant.Seasons,
                    season =>
                    {
                        var (color, bold) = SeasonHelper.Style(season);

                        return new InlineSegment
                        {
                            Text = SeasonHelper.Translate(season),
                            Color = color,
                            Bold = bold
                        };

                    }
                );

                list.Add(new TooltipElement
                {
                    InlineSegments = segments
                });
            }

            return list;

        }

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

            public static (Color color, bool bold) Style(SeasonId season)
            {
                bool isCurrent = season.ToString().Equals(Game1.currentSeason, StringComparison.OrdinalIgnoreCase);

                Color color = season switch
                {
                    SeasonId.Spring => TooltipColors.SpringColor,
                    SeasonId.Summer => TooltipColors.SummerColor,
                    SeasonId.Fall => TooltipColors.FallColor,
                    SeasonId.Winter => TooltipColors.WinterColor,
                    _ => TooltipColors.Normal
                };

                return isCurrent
                    ? (color, true)
                    : (TooltipColors.Normal, false);
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
        }


        //-----------------
        // Plant growth and when it is ready (crops, fruit trees, etc)
        //------------------

        private static List<TooltipElement> GetPlantGrowthTooltip(PlantInfo plant)
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

        //---------------
        // Crops in/out of Season
        //-----------------


        // Display basic data when crops are out of season (trees, bushes treated separately)
        private static List<TooltipElement> CropsOutOfSeason(PlantInfo plant, GrowthContext growth)
        {

            var list = new List<TooltipElement>();

            // Only for Crops that are out of season
            if (plant.PlantType == PlantType.Crop &&
              !plant.Seasons.Contains(growth.CurrentSeason))

            {
                list.Add(new TooltipElement
                {
                    IconTexture = plant.HarvestIconTexture,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.DaysToProduce),
                        growth.ProduceDay
                    ),
                    TextColor = TooltipColors.Normal
                });
                var el = list[list.Count - 1];

                if (plant.RegrowDays > 0)
                {
                    // How many harvests
                    list.Add(new TooltipElement
                    {
                        //Icon = TooltipIcons.Spiral,
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.RegrowQty),
                            growth.RegrowQty),
                        TextColor = TooltipColors.Normal
                    });
                }


            }
            return list;
        }

        // Display ready data when crops are ready this season (trees, bushes treated separately)
        private static List<TooltipElement> CropsReadyThisSeason(PlantInfo plant, GrowthContext growth)
        {

            var list = new List<TooltipElement>();

            if (plant.PlantType == PlantType.Crop &&
              plant.Seasons.Contains(growth.CurrentSeason) &&
              growth.ReadyDay <= 28)

            {
                list.Add(new TooltipElement
                {
                    IconTexture = plant.HarvestIconTexture,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.ReadyOn),
                        growth.ProduceDay,
                        growth.CurrentSeason,
                        growth.ReadyDay
                        ),
                    TextColor = TooltipColors.Normal
                });

                if (plant.RegrowDays > 0)
                {
                    // How many harvests
                    list.Add(new TooltipElement
                    {
                        //Icon = TooltipIcons.Spiral,
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.RegrowQty),
                            growth.RegrowQty),
                        TextColor = TooltipColors.Normal
                    });
                }
            }
            return list;
        }

        // Display ready data when multiseason crops are ready next season (trees, bushes treated separately)
        private static List<TooltipElement> CropsReadyNextSeason(PlantInfo plant, GrowthContext growth)
        {

            var list = new List<TooltipElement>();

            if (plant.PlantType == PlantType.Crop &&
              plant.Seasons.Contains(growth.CurrentSeason) &&
              plant.Seasons.Contains(growth.NextSeason) &&
              growth.ReadyDay >= 28)

            {
                list.Add(new TooltipElement
                {
                    IconTexture = plant.HarvestIconTexture,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.ReadyOn),
                        growth.ProduceDay,
                        growth.NextSeason,
                        growth.OverflowDay),
                    TextColor = TooltipColors.Normal
                });

                if (plant.RegrowDays > 0)
                {
                    // How many harvests
                    list.Add(new TooltipElement
                    {
                        //Icon = TooltipIcons.Spiral,
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.RegrowQty),
                            growth.RegrowQty),
                        TextColor = TooltipColors.Normal
                    });
                }


            }
            return list;
        }

        // Display basic data and a warning when it is too late to plant (trees, bushes treated separately)
        private static List<TooltipElement> CropsTooLateToPlant(PlantInfo plant, GrowthContext growth)
        {

            var list = new List<TooltipElement>();

            if (plant.PlantType == PlantType.Crop &&
              plant.Seasons.Contains(growth.CurrentSeason) &&
              !plant.Seasons.Contains(growth.NextSeason) &&
              growth.ReadyDay >= 28)

            {
                // First show the basic days to produce
                list.Add(new TooltipElement
                {
                    IconTexture = plant.HarvestIconTexture,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.DaysToProduce),
                        growth.ProduceDay),
                    TextColor = TooltipColors.Normal
                });

                // Add the too late warning message
                list.Add(new TooltipElement
                {
                    IconRef = TooltipIcons.Warning,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.TooLate)),
                    TextColor = TooltipColors.Warning
                });

            }
            return list;
        }

        //---------------
        // Paddy Crops in/out of Season
        //-----------------
        // Display basic data when crops are out of season (trees, bushes treated separately)
        private static List<TooltipElement> PaddyCropsOutOfSeason(PlantInfo plant, GrowthContext growth)
        {

            var list = new List<TooltipElement>();

            // Only for Crops that are out of season
            if (plant.PlantType == PlantType.Crop &&
              !plant.Seasons.Contains(growth.CurrentSeason) &&
              plant.Paddy)
            {

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
                    // How many harvests
                    list.Add(new TooltipElement
                    {
                        //Icon = TooltipIcons.Spiral,
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.RegrowQty),
                            growth.PaddyRegrowQty),
                        TextColor = TooltipColors.Paddy
                    });
                }

            }
            return list;
        }

        // Display ready data when crops are ready this season (trees, bushes treated separately)
        private static List<TooltipElement> PaddyCropsReadyThisSeason(PlantInfo plant, GrowthContext growth)
        {

            var list = new List<TooltipElement>();

            if (plant.PlantType == PlantType.Crop &&
              plant.Seasons.Contains(growth.CurrentSeason) &&
              growth.PaddyReadyDay <= 28 &&
              plant.Paddy)

            {
                list.Add(new TooltipElement
                {
                    IconRef = TooltipIcons.WaterSeeds,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.ReadyOn),
                        growth.PaddyProduceDay,
                        growth.CurrentSeason,
                        growth.PaddyReadyDay),
                    TextColor = TooltipColors.Paddy
                });

                if (plant.RegrowDays > 0)
                {
                    // How many harvests
                    list.Add(new TooltipElement
                    {
                        //Icon = TooltipIcons.Spiral,
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.RegrowQty),
                            growth.PaddyRegrowQty),
                        TextColor = TooltipColors.Paddy
                    });

                }
            }
            return list;
        }

        // Display ready data when multiseason crops are ready next season (trees, bushes treated separately)
        private static List<TooltipElement> PaddyCropsReadyNextSeason(PlantInfo plant, GrowthContext growth)
        {

            var list = new List<TooltipElement>();

            if (plant.PlantType == PlantType.Crop &&
              plant.Seasons.Contains(growth.CurrentSeason) &&
              plant.Seasons.Contains(growth.NextSeason) &&
              growth.PaddyReadyDay >= 28 &&
              plant.Paddy)

            {
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
                    // How many harvests
                    list.Add(new TooltipElement
                    {
                        //Icon = TooltipIcons.Spiral,
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.RegrowQty),
                            growth.PaddyRegrowQty),
                        TextColor = TooltipColors.Paddy
                    });
                }

            }
            return list;
        }

        // Display basic data and a warning when it is too late to plant (trees, bushes treated separately)
        private static List<TooltipElement> PaddyCropsTooLateToPlant(PlantInfo plant, GrowthContext growth)
        {

            var list = new List<TooltipElement>();

            if (plant.PlantType == PlantType.Crop &&
              plant.Seasons.Contains(growth.CurrentSeason) &&
              !plant.Seasons.Contains(growth.NextSeason) &&
              growth.PaddyReadyDay >= 28 &&
              plant.Paddy)

            {
                // First show the basic days to produce
                list.Add(new TooltipElement
                {
                    IconRef = TooltipIcons.WaterSeeds,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.DaysToProduce),
                        growth.PaddyProduceDay),
                    TextColor = TooltipColors.Paddy
                });

                // Add the too late warning message
                list.Add(new TooltipElement
                {
                    IconRef = TooltipIcons.Warning,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.TooLate)),
                    TextColor = TooltipColors.Warning
                });

            }
            return list;
        }

        //---------------
        // Fruit Trees in/out of Season
        //-----------------

        private static List<TooltipElement> FruitTreesReadyNextSeason(PlantInfo plant, GrowthContext growth)
        {

            var list = new List<TooltipElement>();

            if (plant.PlantType == PlantType.FruitTree &&
              plant.Seasons.Contains(growth.NextSeason))
            {
                list.Add(new TooltipElement
                {
                    IconTexture = plant.HarvestIconTexture,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.ReadyOn),
                        growth.ProduceDay,
                        growth.NextSeason,
                        growth.ReadyDay
                        ),
                    TextColor = TooltipColors.Normal
                });

                if (plant.RegrowDays > 0)
                {
                    // How many harvests
                    list.Add(new TooltipElement
                    {
                        //Icon = TooltipIcons.Spiral,
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.RegrowQty),
                            growth.RegrowQty),
                        TextColor = TooltipColors.Normal
                    });
                }
            }
            return list;
        }

        private static List<TooltipElement> FruitTreesReadyInFuture(PlantInfo plant, GrowthContext growth)
        {

            var list = new List<TooltipElement>();

            if (plant.PlantType == PlantType.FruitTree &&
              !plant.Seasons.Contains(growth.NextSeason))
            {
                list.Add(new TooltipElement
                {
                    IconTexture = plant.HarvestIconTexture,
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.TreeReadyInFuture),
                        plant.Seasons?.FirstOrDefault()),
                    TextColor = TooltipColors.Normal
                });

                if (plant.RegrowDays > 0)
                {
                    // How many harvests
                    list.Add(new TooltipElement
                    {
                        //Icon = TooltipIcons.Spiral,
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.RegrowQty),
                            growth.RegrowQty),
                        TextColor = TooltipColors.Normal
                    });
                }
            }
            return list;
        }



        private static GrowthContext BuildGrowthContext(PlantInfo plant)
        {
            int today = Game1.dayOfMonth;
            SeasonId currentSeason = SeasonHelper.FromGameSeason(Game1.season);
            SeasonId nextSeason = SeasonHelper.Next(currentSeason);

            int days = plant.DaysToProduce ?? 0;

            int readyDay = today + days;

            int paddyDays = (int)Math.Floor(days / 1.25f);
            int paddyReadyDay = today + paddyDays;

            int regrowDays = plant.RegrowDays.GetValueOrDefault();
            int paddyRegrowDays = (int)Math.Floor(regrowDays / 1.25f);
            int additionalSeasons = SeasonHelper.CountAdditionalSeasons(currentSeason, plant.Seasons);
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
                regrowDaysAvailable = 28 * plant.Seasons.Count - days - 1;
                paddyRegrowDaysAvailable = 28 * plant.Seasons.Count - paddyDays - 1;
            }

            // Regrow days for fruit trees next season
            if (plant.RegrowDays > 0 &&
                plant.Seasons.Contains(nextSeason) &&
                plant.PlantType == PlantType.FruitTree)
            {
                readyDay = today + days - 28 * additionalSeasons;
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


            //ModEntry.Instance.Monitor.Log($"TEXT: {readyDay}", LogLevel.Info);

            // paddyRegrowDaysAvailable = (int)Math.Floor(regrowDaysAvailable / 1.25);
            double r = (double)regrowDaysAvailable / regrowDays;
            regrowQty = 1 + (int)Math.Floor(r);

            double pr = (double)paddyRegrowDaysAvailable / paddyRegrowDays;
            paddyRegrowQty = 1 + (int)Math.Floor(pr);

            return new GrowthContext
            {
                //Today = today,
                CurrentSeason = currentSeason,
                NextSeason = nextSeason,

                //Seasons = seasons,
                ProduceDay = days,
                PaddyProduceDay = paddyDays,

                ReadyDay = readyDay,
                PaddyReadyDay = paddyReadyDay,

                OverflowDay = readyDay - 28,
                PaddyOverflowDay = paddyReadyDay - 28,

                RegrowQty = regrowQty,
                PaddyRegrowQty = paddyRegrowQty,

            };
        }

        //---------------
        // Economics
        //-----------------

        private static List<TooltipElement> GetEconomicsTooltip(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            new InlineSegment
            {
                IconRef = TooltipIcons.LittleCoin
            };

            //----------------
            // Seed purchase
            //----------------

            ////debug piece

            //var pierre = plant.PurchaseOptions
            //    .FirstOrDefault(p => p.VendorId == "SeedShop");

            //if (pierre != null && pierre.GoldPrice.HasValue)
            //{
            //    ModEntry.Instance.Monitor.Log(
            //        $"Pierre price: {pierre.GoldPrice.Value}",
            //        LogLevel.Info
            //    );
            //}
            //else
            //{
            //    ModEntry.Instance.Monitor.Log(
            //        "Pierre price: <none found>",
            //        LogLevel.Info
            //    );
            //}
            ////debug



            bool IsPierre(PurchaseInfo v) =>
                 v.VendorId == "SeedShop";

            bool IsNightMarket(PurchaseInfo v) =>
                v.VendorId.Contains("NightMarket", StringComparison.OrdinalIgnoreCase);

            string VendorKey(PurchaseInfo v)
            {
                if (IsPierre(v))
                    return "SeedShop";      // all Pierre entries collapse to one

                if (IsNightMarket(v))
                    return "NightMarket";   // all Night Market entries collapse to one

                return v.VendorId;          // others stay distinct
            }

            var goldVendors = plant.PurchaseOptions
                .Where(p => p.GoldPrice.HasValue && p.GoldPrice > 0)
                .Where(p => !IgnoredVendors.Contains(p.VendorId))
                .GroupBy(v => VendorKey(v))
                .Select(g => g.First())
                .ToList();


            // Sort the vendor list so Pierre's price outputs first and Night Market is last
            int SortKey(PurchaseInfo v)
            {
                if (IsPierre(v))
                    return 0;

                if (IsNightMarket(v))
                    return 2;

                return 1;
            }

            var sortedVendors = goldVendors
                .OrderBy(v => SortKey(v))
                .ThenBy(v => v.VendorName)
                .ToList();

            var segments = TooltipRenderer.BuildInlineSegments(
                sortedVendors,
                vendor =>
                {
                    if (IsPierre(vendor))
                    {
                        return new InlineSegment
                        {

                            Text = string.Format(
                                ModEntry.ModHelper.Translation.Get(TooltipKeys.PierresPurchase),
                                vendor.GoldPrice),
                            Color = TooltipColors.Normal,
                            Bold = false


                        };
                    }

                    else if (IsNightMarket(vendor))
                    {
                        return new InlineSegment
                        {
                            IconRef = TooltipIcons.NightStars
                        };
                    }
                    else
                    {
                        return new InlineSegment
                        {
                            Text = string.Format(
                                ModEntry.ModHelper.Translation.Get(TooltipKeys.OtherShopPurchase),
                                vendor.GoldPrice,
                                vendor.VendorName),
                            Color = TooltipColors.Normal,
                            Bold = false

                        };
                    }


                });

            list.Add(new TooltipElement
            {
                InlineSegments = segments
            });







            //if (goldVendors.Count == 1)
            //{
            //    var vendor = goldVendors[0];

            //    if (vendor.VendorId == "SeedShop") // Pierre
            //    {
            //        // 20g
            //        list.Add(new TooltipElement
            //        {
            //            IconRef = TooltipIcons.LittleCoin,
            //            Text = string.Format(ModEntry.ModHelper.Translation
            //                               .Get(TooltipKeys.PierresPurchase),
            //                               vendor.GoldPrice),
            //            TextColor = TooltipColors.Normal
            //        });
            //    }
            //    else
            //    {
            //        // Output: "20g at Ari's"
            //        list.Add(new TooltipElement
            //        {
            //            IconRef = TooltipIcons.LittleCoin,
            //            Text = string.Format(ModEntry.ModHelper.Translation
            //               .Get(TooltipKeys.OtherShopPurchase),
            //               vendor.GoldPrice),
            //            TextColor = TooltipColors.Normal
            //        });

            //    }
            //}

            //// Display the relevant seasons and highlight the current season
            //if (plant.PurchaseOptions.GoldPrice > 0)
            //{
            //    var segments = TooltipRenderer.BuildInlineSegments(
            //        plant.Seasons,
            //        season =>
            //        {
            //            var (color, bold) = SeasonHelper.Style(season);

            //            return new InlineSegment
            //            {
            //                Text = SeasonHelper.Translate(season),
            //                Color = color,
            //                Bold = bold
            //            };

            //        }
            //    );

            //    list.Add(new TooltipElement
            //    {
            //        InlineSegments = segments
            //    });


            //----------------
            // How many I have
            //----------------


            //----------------
            // Harvest value
            //----------------
            int harvestBV = plant.HarvestPrice; //Base value of harvest items
                                                //ModEntry.Instance.Monitor.Log($"BV: {harvestBV}", LogLevel.Info);
            int goldStarHarvest = (int)Math.Floor(1.5 * harvestBV); //Value of gold star quality harvest items


            list.Add(new TooltipElement
            {
                IconRef = TooltipIcons.GoldStar,
                Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.PriceRange),
                        harvestBV,
                        goldStarHarvest),
                TextColor = TooltipColors.Normal
            });

            return list;



        }

        private static readonly HashSet<string> IgnoredVendors = new()
            {
                "JojaMart",
                "NightMarket"
            };
    }

}

