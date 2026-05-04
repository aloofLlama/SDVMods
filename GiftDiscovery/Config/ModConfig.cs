using GiftDiscovery.Models;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftDiscovery.Config
{
    public class ModConfig
    {
        public SButton ToggleTooltipKey { get; set; } = SButton.L;

        // Whether to show tastes from game, only the save, or across all saves
        public TasteSourceMode TasteSourceMode { get; set; } = TasteSourceMode.Local;
        public bool HighlightNotMaxFriendship { get; set; } = true;
        public bool DeemphasizeAlreadyGifted { get; set; } = true;

        // Extra options to highlight nearby NPCs
        public bool EmphasizeNearbyNPCs { get; set; } = true;
        public int NearbyRangeTiles { get; set; } = 15; // or whatever default you want


        public bool ShowLoves { get; set; } = true;
        public bool ShowLikes { get; set; } = true;
        public bool ShowNeutral { get; set; } = false;
        public bool ShowDislikes { get; set; } = false;
        public bool ShowHates { get; set; } = false;
        public bool ShowUndiscovered { get; set; } = true;

    }
}
