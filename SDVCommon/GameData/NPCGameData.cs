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
            Texture2D tex = npc.Portrait; // or wherever you get it
            Rectangle src = new Rectangle(0, 0, 64, 64); // portraits are 64×64

            return new Icon(tex, src, size: 64, scale: 1f);
        }
    }
}

