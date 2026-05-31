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

        //-------------------
        //Gift tooltip bottom left
        //-------------------
        public bool HighlightNotMaxFriendship { get; set; } = true;
        public bool DeemphasizeAlreadyGifted { get; set; } = true;
        public bool ShowInCollectionsMenu { get; set; } = true;
        public bool ShowInCoookingMenu { get; set; } = true;
        public bool ShowInCraftingMenu { get; set; } = true;




        // Extra options to highlight nearby NPCs
        public bool EmphasizeNearbyNPCs { get; set; } = true;
        public int NearbyRangeTilesGiftTooltip { get; set; } = 15;
        public int WrapSizeGift { get; set; } = 3; //How many names to show before wrapping


        public bool ShowLoves { get; set; } = true;
        public bool ShowLikes { get; set; } = true;
        public bool ShowNeutral { get; set; } = true;
        public bool ShowDislikes { get; set; } = true;
        public bool ShowHates { get; set; } = true;
        public bool ShowUndiscovered { get; set; } = true;
        public bool ShowModSource { get; set; } = true;


        //-------------------
        //NPC tooltip bottom right
        //-------------------
        public int NearbyRangeTilesNPCTooltip { get; set; } = 3;
        public int WrapSizeNPC { get; set; } = 8; //How many names to show before wrapping
        public int MaxRowsNPC { get; set; } = 8; //How many names to show before wrapping
        public bool ShowLocation { get; set; } = true;
        public bool SeparateUniversalLoves { get; set; } = true;



    }
}
