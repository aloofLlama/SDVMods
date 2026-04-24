using SDVCommon.Models.Wrappers;
using SDVCommon.Icons;
using SDVCommon.Tooltip;
using HarvestHelper.Helpers;
using StardewValley;

namespace HarvestHelper.TooltipSections
{
    internal class FirstSection
    {
        public static List<TooltipElement> Build(HarvestInfo harvest, StardewValley.Object obj)
        {
            var list = new List<TooltipElement>();

            int realPrice = obj.sellToStorePrice();


            list.Add(new TooltipElement
            {
                Icon = TooltipIcons.Get(IconKey.LittleCoin),
                Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.Price),
                        realPrice
                    ),
            });

            return list;


        }
    }
}
