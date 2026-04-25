using PlantingDay.Helpers;
using PlantingDay.Helpers.SeedSource;
using PlantingDay.Models.Runtime;
using PlantingDay.Models.Wrappers;
using SDVCommon;
using SDVCommon.Helpers;
using SDVCommon.Icons;
using SDVData;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDVCommon.Tooltip;


namespace PlantingDay.ToolTip_Sections
{
    // Where to get the seed and how much it costs
    internal class SeedSourceSection
    {
        public static List<TooltipElement> Build(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            // Unified vendor + monster list (already sorted)
            var sources = SeedSourceBuilder.Build(plant);


            // Check for 'rare' drops (<1%)
            bool IsRare(MonsterDropInfoRuntime drop)
            {
                float pct = MathF.Round(drop.Data.DropChance * 100f);
                return pct < 1f;
            }

            var rareDrops = plant.MonsterDrops.Where(IsRare).ToList();
            bool hasMultipleRareDrops = rareDrops.Count > 1;

            var segments = TooltipBuildHelper.BuildInlineSegments(
                sources,
                source =>
                {
                    // Vendors
                    if (source is PurchaseInfoRuntime vendor)
                        return BuildVendorSegments(vendor) ?? Array.Empty<InlineSegment>();

                    // Monsters
                    if (source is MonsterDropInfoRuntime drop)
                    {
                        if (hasMultipleRareDrops)
                        {
                            // Only output rare block ONCE, at the position of the first monster
                            if (drop == plant.MonsterDrops.First())
                                return BuildRareMonsterSegment(plant);

                            return Array.Empty<InlineSegment>();
                        }

                        // Normal output with icon and percentage
                        return BuildMonsterSegments(drop);
                    }

                    return Array.Empty<InlineSegment>();
                });

            // Add 'year 2' flag if applicable
            int minYear = VendorHelper.GetMinYear(plant);
            bool year2Plus = minYear >= 2;

            if (year2Plus)
            {
                segments.Add(new InlineSegment
                {
                    Text = ModEntry.ModHelper.Translation.Get(TooltipKeys.BuyYear2),
                    Color = TooltipColors.Normal
                });
            }

            // Insert seed icon at the front
            if (plant.Runtime.SeedIcon != null)
            {
                segments.Insert(0, new InlineSegment
                {
                    Icon = plant.Runtime.SeedIcon
                });
            }


            list.Add(new TooltipElement
            {
                InlineSegments = segments
            });

            return list;
        }

        //public static List<TooltipElement> Build(PlantInfo plant)
        //{
        //    var list = new List<TooltipElement>();

        //    // Unified vendor + monster list (already sorted)
        //    var sources = SeedSourceBuilder.Build(plant);

        //    var segments = TooltipRenderer.BuildInlineSegments(
        //        sources,
        //        source =>
        //        {
        //            if (source is PurchaseInfoRuntime vendor)
        //                return BuildVendorSegments(vendor) ?? Array.Empty<InlineSegment>();

        //            if (source is MonsterDropInfoRuntime drop)
        //                return BuildMonsterSegments(plant) ?? Array.Empty<InlineSegment>();

        //            return Array.Empty<InlineSegment>();
        //        });

        //    if (plant.Runtime.SeedIcon != null)
        //    {
        //        segments.Insert(0, new InlineSegment
        //        {
        //            Icon = plant.Runtime.SeedIcon
        //        });
        //    }

        //    list.Add(new TooltipElement
        //    {
        //        InlineSegments = segments
        //    });

        //    return list;
        //}

        //──────────────────────────────────────────────
        // Vendor → InlineSegments
        //──────────────────────────────────────────────
        private static InlineSegment[]? BuildVendorSegments(PurchaseInfoRuntime vendor)
        {
            var data = vendor.Data;


            switch (data.Type)
            {
                case PurchaseInfoData.VendorType.Pierre:
                    return new[]
                    {
                        new InlineSegment
                        {
                            Text = string.Format(
                                ModEntry.ModHelper.Translation.Get(TooltipKeys.PierresPurchase),
                                data.GoldPrice),
                            Color = TooltipColors.Normal
                        }
                    };

                case PurchaseInfoData.VendorType.Joja:
                    return null;

                case PurchaseInfoData.VendorType.ValleyFair:
                    return new[]
                    {
                        new InlineSegment
                        {
                            Icon = TooltipIcons.Get(IconKey.StarToken),
                            Text = string.Format(
                                    ModEntry.ModHelper.Translation.Get(TooltipKeys.OtherShopTrade),
                                    data.GoldPrice,
                                    data.VendorName),

                            Color = TooltipColors.Normal
                        }
                    };

                case PurchaseInfoData.VendorType.NightMarket:
                    return new[]
                    {
                        new InlineSegment
                        {
                            Icon = TooltipIcons.Get(IconKey.NightStars)
                        }
                    };

                case PurchaseInfoData.VendorType.TravelingCart:
                    return new[]
                    {
                        new InlineSegment
                        {
                            Icon = TooltipIcons.Get(IconKey.TravelingCart)
                        }
                    };

                default:
                    // Gold vendors
                    if (data.GoldPrice.HasValue)
                    {
                        return new[]
                        {
                            new InlineSegment
                            {
                                Text = string.Format(
                                    ModEntry.ModHelper.Translation.Get(TooltipKeys.OtherShopPurchase),
                                    data.GoldPrice,
                                    data.VendorName),
                                Color = TooltipColors.Normal
                            }
                        };
                    }

                    // Trade vendors
                    if (data.TradeAmount > 0)
                    {
                        return new[]
                        {
                            new InlineSegment
                            {
                                Icon = vendor.CurrencyIcon,
                                Text = string.Format(
                                    ModEntry.ModHelper.Translation.Get(TooltipKeys.OtherShopTrade),
                                    data.TradeAmount.ToString(),
                                    data.VendorName),
                                Color = TooltipColors.Normal
                            }
                        };
                    }

                    return null;
            }
        }

        //──────────────────────────────────────────────
        // Monster Drop → InlineSegments
        //──────────────────────────────────────────────
        private static InlineSegment[] BuildMonsterSegments(MonsterDropInfoRuntime drop)
        {
            return new[]
            {
                new InlineSegment
                {
                    Icon = drop.MonsterIcon,
                    Text = FormatChance(drop.Data.DropChance),
                    Color = TooltipColors.Normal
                }
            };
        }

        private static InlineSegment[] BuildRareMonsterSegment(PlantInfo plant)
        {
            bool IsRare(MonsterDropInfoRuntime d)
            {
                float pct = MathF.Round(d.Data.DropChance * 100f);
                return pct < 1f;
            }

            var rareDrops = plant.MonsterDrops
                .Where(IsRare)
                .ToList();

            var segments = new List<InlineSegment>();

            for (int i = 0; i < rareDrops.Count; i++)
            {
                var drop = rareDrops[i];
                bool isLast = (i == rareDrops.Count - 1);

                segments.Add(new InlineSegment
                {
                    Icon = drop.MonsterIcon,
                    Text = isLast ? "rare" : "",
                    Color = TooltipColors.Normal
                });
            }

            return segments.ToArray();
        }


        public static string FormatChance(float chance)
        {
            if (chance <= 0f)
                return "0%";

            float pct = chance * 100f;

            // Round first to avoid float precision issues
            pct = MathF.Round(pct);

            if (pct < 1f)
                return "rare"; // cannot use < or > with SDV text renderer

            return $"{pct}%";
        }

    }
}
