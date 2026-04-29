using SDVCommon.Models.Wrappers;
using SDVCommon.Icons;
using SDVCommon.Tooltip;
using SDVCommon.Helpers;
using HarvestHelper.Helpers;
using StardewValley;
using Microsoft.Xna.Framework;


namespace HarvestHelper.TooltipSections
{
    public static class FirstSection
    {
        public static List<TooltipElement> Build(HarvestInfo harvest, StardewValley.Object obj)
        {
            var list = new List<TooltipElement>();

            int singlePrice = obj.sellToStorePrice();
            int stack = obj.Stack;
            int totalPrice = singlePrice * stack;

            var segs = new List<InlineSegment>();

            // always show single price
            segs.Add(new InlineSegment
            {
                Text = $"{singlePrice} ",
            });

            // only show total price if stack > 1
            if (stack > 1)
            {
                //segs.Add(new InlineSegment
                //{
                //    Text = " ", // just a space between them
                //});

                segs.Add(new InlineSegment
                {
                    Text = $"[{totalPrice}]",
                    TextColor = TooltipColors.Deemphasize
                    //TextColor = Color.DarkSlateGray
                });
            }

            list.Add(new TooltipElement
            {
                Icon = TooltipIcons.Get(IconKey.LittleCoin),
                InlineSegments = segs
            });

            return list;
        }
    }
}
