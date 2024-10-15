using StardewValley;
using StardewValley.Buildings;


namespace FarmAnimalHarvestHelper
{
    /// <summary>
    /// Version 1.6.9 specific
    /// </summary>
    public class VersionCode
    {
        public static Building GetParentBuilding(GameLocation house)
        {
            return house.ParentBuilding;
        }
    }
}
