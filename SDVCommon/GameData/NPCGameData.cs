using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon.Icons;
using StardewValley;

namespace SDVCommon.GameData
{
    public static class NPCGameData
    {
        public static Icon GetPortraitIcon(NPC npc)
        {
            int portraitSize = 64;
            Texture2D tex = npc.Portrait; 
            Rectangle src = new Rectangle(0, 0, portraitSize, portraitSize);

            return new Icon(tex, src, portraitSize);
        }
    }
}

