//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using StardewValley;
//using System;

//namespace SDVCommon.Icons.IconProviders
//{
//    public class MonsterIconProvider : IIconProvider
//    {
//        public bool CanHandle(string id)
//            => id.StartsWith("monster:", StringComparison.OrdinalIgnoreCase);

//        public Icon? LoadIcon(string id)
//        {
//            string monsterName = id["monster:".Length..];

//            try
//            {
//                // Let the game construct the monster
//                var monster = Utility.getMonsterFromName(monsterName, 0, 0);
//                if (monster == null || monster.Sprite == null)
//                    return null;

//                Texture2D tex = monster.Sprite.Texture;
//                Rectangle source = monster.Sprite.SourceRect;

//                // Normalize to something icon-ish; you can tweak size/scale
//                int size = Math.Max(source.Width, source.Height);

//                return new Icon(
//                    tex,
//                    source,
//                    size: size,
//                    scale: 2f
//                );
//            }
//            catch
//            {
//                return null;
//            }
//        }
//    }
//}
