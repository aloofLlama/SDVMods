using StardewValley;



namespace SDVCommon.Helpers
{
    public class InventoryHelper
    {

        public static int CountInInventory(string canonicalId)
        {
            int total = 0;

            foreach (var item in Game1.player.Items)
            {
                if (item is StardewValley.Object obj)
                {
                    string id = IdHelper.CanonicalItemId(obj.QualifiedItemId);

                    if (id == canonicalId)
                        total += obj.Stack;
                }
            }

            return total;
        }

    }
}
