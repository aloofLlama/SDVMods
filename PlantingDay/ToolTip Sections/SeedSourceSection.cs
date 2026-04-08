using PlantingDay.Helpers;
using PlantingDay.Models;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantingDay.ToolTip_Sections
{
    // Where to get the seed and how much it costs
    internal class SeedSourceSection
    {
        public static List<TooltipElement> Build(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            // 1. Get the final ordered list (Pierre → Gold → Trade → Monster → Night Market)
            var sources = SeedSourceAggregator.BuildFullSourceList(plant)
                .Where(s => s is PurchaseInfo || s is MonsterDropInfo)
                .ToList();



            ModEntry.Instance.Monitor.Log("=== INLINE SOURCE ITEMS ===", LogLevel.Warn);
            ModEntry.Instance.Monitor.Log($"Count = {sources.Count}", LogLevel.Warn);
            int idx = 0;
            foreach (var s in sources)
            {
                if (s is PurchaseInfo p)
                {
                    ModEntry.Instance.Monitor.Log(
                        $"[{idx}] PurchaseInfo: Vendor={p.VendorName}, Price={p.GoldPrice}, Trade={p.TradeAmount}, IsNightMarket={VendorHelper.IsNightMarket(p)}",
                        LogLevel.Warn
                    );
                }
                else if (s is MonsterDropInfo m)
                {
                    ModEntry.Instance.Monitor.Log(
                        $"[{idx}] MonsterDrop: Monster={m.MonsterName}, Chance={m.Chance}",
                        LogLevel.Warn
                    );
                }
                else
                {
                    ModEntry.Instance.Monitor.Log(
                        $"[{idx}] UNKNOWN TYPE: {s?.GetType().Name}",
                        LogLevel.Warn
                    );
                }

                idx++;
            }







            // 2. Convert each source into inline segments
            var segments = TooltipRenderer.BuildInlineSegments(
                sources,
                source =>
                {
                    //ModEntry.Instance.Monitor.Log($"Selector invoked for {source.GetType().Name}", LogLevel.Warn);

                    if (source is PurchaseInfo vendor)
                        return BuildVendorSegments(vendor) ?? Array.Empty<InlineSegment>();


                    if (source is MonsterDropInfo drop)
                        return BuildMonsterSegments(drop) ?? Array.Empty<InlineSegment>();


                    return Array.Empty<InlineSegment>();

                });
            //for (int i = 0; i < segments.Count; i++)
            //{
            //    ModEntry.Instance.Monitor.Log(
            //        $"SEGMENT[{i}]: IconTexture={segments[i].IconTexture != null}, IconRef={segments[i].IconRef}, Text='{segments[i].Text}'",
            //        LogLevel.Info
            //    );
            //}

            // 3. Add seed icon at the start
            if (plant.SeedIconRef != null)
            {
                segments.Insert(0, new InlineSegment
                {
                    IconRef = plant.SeedIconRef,
                });
//                ModEntry.Instance.Monitor.Log(
//    $"SEGMENT[0] after insert: IconTexture={segments[0].IconTexture != null}, Text='{segments[0].Text}'",
//    LogLevel.Info
//);

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
            // Pierre (gold only)
            if (VendorHelper.IsPierre(vendor))
            {
                return new[]
                {
                new InlineSegment
                {
                    Text = string.Format(
                        ModEntry.ModHelper.Translation.Get(TooltipKeys.PierresPurchase),
                        vendor.GoldPrice),
                    Color = TooltipColors.Normal
                }
            };
            }

            // Night Market (icon only)
            if (VendorHelper.IsNightMarket(vendor))
            {
                return new[]
                {
                new InlineSegment
                {
                    IconRef = TooltipIcons.NightStars
                }
            };
            }

            // Valley Fair (star tokens)
            if (VendorHelper.IsValleyFair(vendor))
            {
                return new[]
                {
                new InlineSegment
                {
                    IconRef = TooltipIcons.StarToken,
                    Text = $" {vendor.GoldPrice} at {vendor.VendorName}",
                    Color = TooltipColors.Normal
                }
            };
            }

            // Gold vendors
            if (vendor.GoldPrice.HasValue && 
                !VendorHelper.IsNightMarket(vendor)) // night market handled above
            {
                return new[]
                {
                new InlineSegment
                {
                    Text = string.Format(
                        ModEntry.ModHelper.Translation.Get(TooltipKeys.OtherShopPurchase),
                        vendor.GoldPrice,
                        vendor.VendorName),
                    Color = TooltipColors.Normal
                }
            };
            }

            // Trade vendors
            if (vendor.TradeAmount > 0)
            {
                return new[]
                {
                new InlineSegment
                {
                    IconRef = vendor.CurrencyIconRef,
                    Text = string.Format(
                        ModEntry.ModHelper.Translation.Get(TooltipKeys.OtherShopTrade),
                        vendor.TradeAmount.ToString(),
                        vendor.VendorName),
                    Color = TooltipColors.Normal
                }
            };
            }

            // Fallback
            return null;
        }

        //──────────────────────────────────────────────
        // Monster Drop → InlineSegments
        //──────────────────────────────────────────────
        private static InlineSegment[]? BuildMonsterSegments(MonsterDropInfo drop)
        {
            return new[]
            {
            new InlineSegment
            {
                IconRef = drop.MonsterIconRef,
                Text = $" {(drop.Chance * 100f):0.#}%",
                Color = TooltipColors.Normal
            }

            };

        }
    }


    //internal class SeedSourceSection
    //{
    //    public static List<TooltipElement> Build(PlantInfo plant)
    //    {
    //        var list = new List<TooltipElement>();
















    //        var sources = new List<object>();
    //        sources.AddRange(sortedVendors);
    //        sources.AddRange(plant.MonsterDrops);


    //        var segments = TooltipRenderer.BuildInlineSegments(
    //            sources,
    //            source =>
    //            {
    //                if (source is PurchaseInfo vendor)
    //                {
    //                    // Pierre (gold only)
    //                    if (IsPierre(vendor))
    //                    {
    //                        return new[]
    //                        {
    //                            new InlineSegment
    //                            {

    //                                Text = string.Format(
    //                                    ModEntry.ModHelper.Translation.Get(TooltipKeys.PierresPurchase),
    //                                    vendor.GoldPrice),
    //                                Color = TooltipColors.Normal,
    //                                Bold = false
    //                            }
    //                        };
    //                    }

    //                    // Night Market (icon only)
    //                    else if (IsNightMarket(vendor))
    //                    {
    //                        return new[]
    //                        {
    //                                new InlineSegment
    //                                {
    //                                    IconRef = TooltipIcons.NightStars
    //                                }
    //                        };
    //                    }

    //                    // Valley Fair (star tokens)
    //                    else if (IsValleyFair(vendor))
    //                    {
    //                        return new[]
    //                        {
    //                            new InlineSegment
    //                            {
    //                                IconRef = TooltipIcons.StarToken,
    //                                Text = $" {vendor.GoldPrice} at {vendor.VendorName}",
    //                                Color = TooltipColors.Normal
    //                            }
    //                        };
    //                    }

    //                    // Other Shops (gold purchase)
    //                    else if (vendor.GoldPrice.HasValue)
    //                    {
    //                        return new[]
    //                        {
    //                            new InlineSegment
    //                            {
    //                                Text = string.Format(
    //                                    ModEntry.ModHelper.Translation.Get(TooltipKeys.OtherShopPurchase),
    //                                    vendor.GoldPrice,
    //                                    vendor.VendorName),
    //                                Color = TooltipColors.Normal,
    //                                Bold = false
    //                            }
    //                        };
    //                    }

    //                    // Other Shops (trade price)
    //                    else if (vendor.TradeAmount > 0)
    //                    {
    //                        return new[]
    //                        {
    //                            new InlineSegment
    //                            {
    //                                IconRef = vendor.CurrencyIconRef,
    //                                Text = string.Format(
    //                                    ModEntry.ModHelper.Translation.Get(TooltipKeys.OtherShopTrade),
    //                                    vendor.TradeAmount.ToString(),
    //                                    vendor.VendorName),
    //                                Color = TooltipColors.Normal,
    //                                Bold = false
    //                            }
    //                        };
    //                    }

    //                    // Fallback for vendor
    //                    return Array.Empty<InlineSegment>();
    //                }

    //                if (source is MonsterDropInfo drop)
    //                {
    //                    return new[]
    //                    {
    //                        new InlineSegment
    //                        {
    //                            IconRef = drop.MonsterIconRef,
    //                            Text = $" {(drop.Chance * 100f):0.#}%",
    //                            Color = TooltipColors.Normal
    //                        }
    //                    };
    //                }

    //                // Fallback for unknown source type
    //                return Array.Empty<InlineSegment>();
    //            });

    //        // Coin icon at start of row if there are gold purchases
    //        if (sortedVendors.Any(v => v.GoldPrice.HasValue))
    //        {
    //            segments.Insert(0, new InlineSegment
    //            {
    //                IconRef = TooltipIcons.LittleCoin
    //            });
    //        }

    //        list.Add(new TooltipElement
    //        {
    //            InlineSegments = segments
    //        });
    //    }






    //}
}
