using PlantingDay.Helpers;
using SDVCommon.Models.Tooltip;
using SDVCommon.Models.Wrappers;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.GameData;
using SDVCommon.Models.Builders;
using SDVCommon.Icons;
using SDVData;
using SDVCommon.Models.Runtime;


namespace PlantingDay.ToolTip_Sections
{
    // Where to get the seed and how much it costs
    internal static class SeedSourceSection
    {
        public static List<TooltipElement> Build(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            // Unified vendor + monster list (already sorted)
            var sources = SeedSourceBuilder.Build(plant);


            // Check for 'rare' drops (<1%)
            static bool IsRare(MonsterDropInfo drop)
            {
                float pct = MathF.Round(drop.Data.DropChance * 100f);
                return pct < 1f;
            }

            var rareDrops = plant.MonsterDrops.Where(IsRare).ToList();
            bool hasMultipleRareDrops = rareDrops.Count > 1;

            var segments = TooltipBuildHelper.BuildInlineSegmentswithSeparators(
                sources,
                source =>
                {
                    // Vendors
                    if (source is PurchaseInfo vendor)
                        return BuildVendorSegments(vendor) ?? Array.Empty<InlineSegment>();

                    // Monsters
                    if (source is MonsterDropInfo drop)
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
            int minYear = Vendor.GetMinYear(plant);
            bool year2Plus = minYear >= 2;

            if (year2Plus)
            {
                segments.Add(new InlineSegment
                {
                    Text = ModEntry.ModHelper.Translation.Get(TooltipKeys.BuyYear2),
                    
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


        //──────────────────────────────────────────────
        // Vendor → InlineSegments
        //──────────────────────────────────────────────
        private static InlineSegment[]? BuildVendorSegments(PurchaseInfo vendor)
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
                            
                        }
                    };

                case PurchaseInfoData.VendorType.Joja:
                    return null;

                case PurchaseInfoData.VendorType.JojaEmporium: //SDV Expanded
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
                                Icon = vendor.Runtime.CurrencyIcon,
                                Text = string.Format(
                                    ModEntry.ModHelper.Translation.Get(TooltipKeys.OtherShopTrade),
                                    data.TradeAmount.ToString(),
                                    data.VendorName),
                                
                            }
                        };
                    }

                    return null;
            }
        }

        //──────────────────────────────────────────────
        // Monster Drop → InlineSegments
        //──────────────────────────────────────────────
        private static InlineSegment[] BuildMonsterSegments(MonsterDropInfo drop)
        {
            return new[]
            {
                new InlineSegment
                {
                    Icon = drop.Runtime.MonsterIcon,
                    Text = FormatChance(drop.Data.DropChance),
                    
                }
            };
        }

        private static InlineSegment[] BuildRareMonsterSegment(PlantInfo plant)
        {
            static bool IsRare(MonsterDropInfo d)
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
                    Icon = drop.Runtime.MonsterIcon,
                    Text = isLast ? "rare" : "",
                    
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
