using HarmonyLib;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.FarmAnimals;
using StardewValley.TokenizableStrings;
using StardewValley;
using SDObject = StardewValley.Object;


namespace FarmAnimalHarvestHelper.Patches
{
    internal class UtilityPatches
    {
        /// <summary>
        /// Changes logic to allow purchasing base animals (cow and chicken)
        /// without a vanilla Barn
        /// </summary>
        [HarmonyPatch(typeof(Utility), nameof(Utility.getPurchaseAnimalStock))]
        public class Utility_getPurchaseAnimalStock_Patch
        {
            private static bool HasBuildingOrUpgrade(GameLocation location, string buildingId)
            {
                if (location.getNumberBuildingsConstructed(buildingId) > 0)
                {
                    return true;
                }
                foreach (KeyValuePair<string, BuildingData> pair in Game1.buildingData)
                {
                    string curId = pair.Key;
                    BuildingData building = pair.Value;
                    if (!(curId == buildingId) && building.BuildingToUpgrade == buildingId && HasBuildingOrUpgrade(location, curId))
                    {
                        return true;
                    }
                }
                return false;
            }
            private static bool HaveRequiredBuilding(GameLocation location, string? requiredBuildType, string homeType)
            {
                //
                //  check for required building
                //
                bool haveRequired = string.IsNullOrEmpty(requiredBuildType);
                if (!haveRequired)
                {
                    haveRequired = HasBuildingOrUpgrade(location, requiredBuildType);
                }
                //
                //  check ValidOccupants values
                //
                if (haveRequired)
                {
                    return location.buildings.Where(p => p.GetData().ValidOccupantTypes != null && p.GetData().ValidOccupantTypes.Contains(homeType)).Any();
                }

                return false;
            }
            public static bool Prefix(GameLocation location, ref List<SDObject> __result)
            {
                if (!ModEntry.Config.EnableFarmAnimalFix)
                    return true;

                __result = new List<SDObject>();
                foreach (KeyValuePair<string, FarmAnimalData> pair in Game1.farmAnimalData)
                {
                    FarmAnimalData data = pair.Value;
                    if (data.PurchasePrice >= 0 && GameStateQuery.CheckConditions(data.UnlockCondition))
                    {
                        SDObject o = new("100", 1, isRecipe: false, data.PurchasePrice)
                        {
                            Name = pair.Key,
                            Type = null
                        };
                        //
                        // remove game requirement for vanilla building required
                        //  
                        string? requiredBuilding = data.RequiredBuilding;
                        if (requiredBuilding != null && requiredBuilding == data.House)
                            requiredBuilding = null;

                        if (!HaveRequiredBuilding(location, requiredBuilding, data.House))
                        {
                            o.Type = ((data.ShopMissingBuildingDescription == null) ? "" : TokenParser.ParseText(data.ShopMissingBuildingDescription));
                        }
                        o.displayNameFormat = data.ShopDisplayName;
                        __result.Add(o);
                    }
                }

                return false;
            }
        }
    }
}
