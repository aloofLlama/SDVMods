
using StardewValley;

namespace SDVCommon.GameData.Dynamic
{
    internal class MenuMonitor
    {
        private static bool _lastHudVisible;
        private static bool _lastMenuVisible;

        public static bool MenuStateChanged()
        {
            bool hud = Game1.displayHUD;
            bool menu = Game1.activeClickableMenu != null;
            if (hud != _lastHudVisible || menu != _lastMenuVisible)
            {
                _lastHudVisible = hud;
                _lastMenuVisible = menu;

                return true;
            }

            return false;
        }



    }

}
