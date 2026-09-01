using GiftDiscovery.GameData.Dynamic;
using GiftDiscovery.Helpers;
using SDVCommon.GameData;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Icons;
using SDVCommon.Icons.iconproviders;
using SDVCommon.Models.Tooltip;
using StardewValley;

namespace GiftDiscovery.Tooltip.NPCSections
{
    internal class NPCHeader
    {
        internal static TooltipElement Build(NPC npc)
        {
            var portrait = NPCGameData.GetPortraitIcon(npc, 0.7f);

            // Add portrait and NPC name
            var segments = new List<InlineSegment>
            {
                new() {
                    Icon = portrait,
                    Text = " " + npc.displayName + "   ",
                    TextColor = DisplayHelper.GetNPCNameColor(npc),
                    Bold = true
                }
            };

            // Add hearts if not maxed out
            int current = HeartStatus.GetCurrentHearts(npc);
            int max = HeartStatus.GetMaxHearts(npc);
            bool isMax = HeartStatus.IsMaxHearts(npc);

            if (!isMax)
            {
                segments.Add(new InlineSegment
                {
                    Icon = IconKey.Heart.GetIcon(),
                    Text = $"{current}/{max}   ",
                    TextColor = TooltipColors.Perfection
                });
            }

            // Add present icon on birthday
            if(npc.isBirthday())
            {
                segments.Add(new InlineSegment
                {
                    Icon = IconKey.Present.GetIcon(),
                });
            }


            return new TooltipElement
            {
                InlineSegments = segments
            };
        }
    }
}