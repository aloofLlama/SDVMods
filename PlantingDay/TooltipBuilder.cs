using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlantingDay;
using PlantingDay.Helpers;
using PlantingDay.Helpers.Icons;
using PlantingDay.Models;
using PlantingDay.ToolTip_Sections;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Crops;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static StardewValley.Menus.CharacterCustomization;
using static System.Net.Mime.MediaTypeNames;



namespace PlantingDay
{
    public static class TooltipBuilder
    {
        public static List<TooltipElement> BuildTooltip(PlantInfo plant)
        {
            var list = new List<TooltipElement>();

            AddIfNotNull(list, SeasonSection.Build(plant));
            list.AddRange(PlantGrowthSection.Build(plant));

            //list.Add(new TooltipElement { IsSeparator = true, PaddingTop = 3, PaddingBottom = 3 });
            AddSectionWithSeparator(list, () => PlantFeaturesSection.Build(plant));
            AddSectionWithSeparator(list, () => SeedSourceSection.Build(plant));




            ////NOT YET REFACTORED BELOW HERE. PULLING EACH SECTION TO OWN FILE




            list.Add(new TooltipElement { IsSeparator = true, PaddingTop = 6, PaddingBottom = 6 });

            list.AddRange(GetEconomicsTooltip(plant));

            return list;
        }
        private static void AddIfNotNull(List<TooltipElement> list, TooltipElement? element)
        {
            if (element != null && IsVisible(element))
                list.Add(element);
        }

        private static bool SectionHasVisibleContent(IEnumerable<TooltipElement> section)
        {
            return section.Any(IsVisible);
        }

        private static bool IsVisible(TooltipElement e)
        {
            bool hasInline = e.InlineSegments != null &&
                             e.InlineSegments.Any(seg =>
                                 !string.IsNullOrWhiteSpace(seg.Text) ||
                                 seg.Icon != null);

            return
                hasInline ||
                !string.IsNullOrWhiteSpace(e.Text) ||
                e.IconTexture != null ||
                e.IsSeparator;
        }

        private static void AddSectionWithSeparator(
            List<TooltipElement> list,
            Func<IEnumerable<TooltipElement>> sectionBuilder,
            int paddingTop = 3,
            int paddingBottom = 3
            )
            {
            var section = sectionBuilder()?.ToList();
            if (section == null || !SectionHasVisibleContent(section))
                return;

            if (list.Count > 0)
            {
                list.Add(new TooltipElement
                {
                    IsSeparator = true,
                    PaddingTop = paddingTop,
                    PaddingBottom = paddingBottom
                });
            }

            foreach (var element in section)
                AddIfNotNull(list, element);
        }









        //---------------
        // Economics
        //-----------------

        private static List<TooltipElement> GetEconomicsTooltip(PlantInfo plant)
        {
            var list = new List<TooltipElement>();


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






            //----------------
            // How many I have
            //----------------


            //----------------
            // Harvest value
            //----------------

            //TODO: Try this command that UI Info Suite Alt 2 uses:    return GetHarvest(item)?.sellToStorePrice() ?? 0;
            int harvestBV = plant.Data.HarvestPrice; //Base value of harvest items
            //ModEntry.Instance.Monitor.Log($"BV: {harvestBV}", LogLevel.Info);
            int goldStarHarvest = (int)Math.Floor(1.5 * harvestBV); //Value of gold star quality harvest items


            list.Add(new TooltipElement
            {
                Icon = TooltipIcons.GoldStar,
                Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.PriceRange),
                        harvestBV,
                        goldStarHarvest),
                TextColor = TooltipColors.Normal
            });

            return list;



        }

    }

}

