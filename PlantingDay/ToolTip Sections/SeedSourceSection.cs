using PlantingDay.Helpers;
using SDVCommon.Icons;
using PlantingDay.Helpers.SeedSource;
using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;
using PlantingDay.Models.Runtime;
using PlantingDay.Models.Wrappers;
using SDVCommon.Helpers;


namespace PlantingDay.ToolTip_Sections
{
    // Where to get the seed and how much it costs
    internal class SeedSourceSection
    {
        public static List<TooltipElement> Build(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            var sources = SeedSourceAggregator.BuildFullSourceList(plant);

            //foreach (var p in PlantDatabase.AllPlants)
            //{
            //    foreach (var option in p.Data.PurchaseOptions)
            //    {
            //        ModEntry.Instance.Monitor.Log(
            //            $"Seed: {p.Data.SeedId} Vendor: {option.VendorName} Price: {option.GoldPrice}",
            //            LogLevel.Warn
            //        );
            //    }
            //}


            var segments = SDVCommon.TooltipRenderer.BuildInlineSegments(
                sources,
                source =>
                {
                    if (source is PurchaseInfoRuntime vendor)
                        return BuildVendorSegments(vendor) ?? System.Array.Empty<InlineSegment>();

                    if (source is MonsterDropInfoRuntime drop)
                        return BuildMonsterSegments(drop) ?? System.Array.Empty<InlineSegment>();

                    return System.Array.Empty<InlineSegment>();
                });

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
        private static InlineSegment[]? BuildVendorSegments(PurchaseInfoRuntime vendor)
        {
            var data = vendor.Data;

            // Pierre (gold only)
            if (VendorHelper.IsPierre(data.VendorId))
            {
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
            }

            // Night Market (icon only)
            if (VendorHelper.IsNightMarket(data.VendorId))
            {
                return new[]
                {
                    new InlineSegment
                    {
                        Icon = TooltipIcons.NightStars
                    }
                };
            }

            // Valley Fair (star tokens)
            if (VendorHelper.IsValleyFair(data.VendorId))
            {
                return new[]
                {
                    new InlineSegment
                    {
                        Icon = TooltipIcons.StarToken,
                        Text = $" {data.GoldPrice} at {data.VendorName}",
                        Color = TooltipColors.Normal
                    }
                };
            }

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

            // Fallback
            return null;
        }

        //──────────────────────────────────────────────
        // Monster Drop → InlineSegments
        //──────────────────────────────────────────────
        private static InlineSegment[]? BuildMonsterSegments(MonsterDropInfoRuntime drop)
        {
            var data = drop.Data;

            return new[]
            {
                new InlineSegment
                {
                    Icon = drop.MonsterIcon,
                    Text = $" {(data.DropChance * 100f):0.#}%",
                    Color = TooltipColors.Normal
                }
            };
        }
    }
}
