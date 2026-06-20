using GiftDiscovery.Helpers;
using SDVCommon.GameData;
using SDVCommon.Helpers.Tooltip;
using SDVCommon.Icons;
using SDVCommon.Models.Tooltip;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftDiscovery.Tooltip.NPCSections
{
    internal class NPCHeader
    {
        public static TooltipElement Build(NPC npc)
        {
            var portrait = NPCGameData.GetPortraitIcon(npc);
            portrait = new Icon(portrait.Texture, portrait.Source, portrait.Size, scale: 0.9f);

            var segments = new List<InlineSegment>();

            segments.Add(new InlineSegment
            {
                Icon = portrait,
                Text = " " + npc.displayName + "   ",
                TextColor = DisplayHelper.GetNPCNameColor(npc),
                Bold = true
            });

            int current = HeartStatus.GetCurrentHearts(npc);
            int max = HeartStatus.GetMaxHearts(npc);
            bool isMax = HeartStatus.IsMaxHearts(npc);

            if (!isMax)
            {
                segments.Add(new InlineSegment
                {
                    Icon = IconKey.Heart.GetIcon(),
                    Text = $"{current}/{max}",
                    TextColor = TooltipColors.Perfection
                });
            }

            return new TooltipElement
            {
                InlineSegments = segments
            };
        }
    }
}