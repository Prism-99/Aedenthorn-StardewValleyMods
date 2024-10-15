using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley;

namespace FarmAnimalHarvestHelper.Patches
{
    internal class FarmAnimalPatches
    {
        /// <summary>
        /// Intercept animal motion to keep animal at holding spot
        /// </summary>
        [HarmonyPatch(typeof(FarmAnimal), nameof(FarmAnimal.updateWhenCurrentLocation))]
        public class FarmAnimal_updateWhenCurrentLocation_Patch
        {
            public static void Postfix(FarmAnimal __instance, GameLocation location)
            {
                if (ModEntry.Config.ModEnabled && Game1.IsMasterGame)
                {
                    if (SlotManager.TryGetHoldVector(__instance, location, out SlotManager.HoldingSpot? holdSpot))
                    {
                        SlotManager.WarpAnimalToHoldingSpot(holdSpot, __instance);
                    }
                    else if (ModEntry.Config.EnableAnimalCycling && SlotManager.IsHoldingActive() && !string.IsNullOrEmpty(__instance.currentProduce.Value))
                    {
                        SlotManager.SlotAnimal(__instance);
                    }
                }
            }
        }
        /// <summary>
        /// Intercept animal motion to keep animal at holding spot
        /// </summary>
        [HarmonyPatch(typeof(FarmAnimal), nameof(FarmAnimal.updateWhenNotCurrentLocation))]
        public class FarmAnimal_updateWhenNotCurrentLocation_Patch
        {
            public static bool Prefix(FarmAnimal __instance, GameLocation environment)
            {
                if (ModEntry.Config.ModEnabled && Game1.IsMasterGame)
                {
                    if (SlotManager.TryGetHoldVector(__instance, environment, out SlotManager.HoldingSpot? holdingSpot))
                    {
                        SlotManager.WarpAnimalToHoldingSpot(holdingSpot, __instance);
                        __instance.updateEmote(Game1.currentGameTime);
                        return false;
                    }
                    if (ModEntry.Config.EnableAnimalCycling && SlotManager.IsHoldingActive() && !string.IsNullOrEmpty(__instance.currentProduce.Value))
                    {
                       return !SlotManager.SlotAnimal(__instance);
                    }                    
                }
                return true;
            }
        }
        /// <summary>
        /// Add logic required to restrict Barn to a single animal
        /// </summary>
        [HarmonyPatch(typeof(FarmAnimal), nameof(FarmAnimal.CanLiveIn))]
        public class FarmAnimal_CanLiveHere
        {
            public static bool Prefix(FarmAnimal __instance, Building building, ref bool __result)
            {
                if (!ModEntry.Config.EnableExactAnimals || building?.GetData() == null) return true;

                __result = false;

                BuildingData? buildingData = building?.GetData();

                if (buildingData != null)
                {
                    if (buildingData.ModData.TryGetValue(ModEntry.modDataKey_useExactHouseRules, out string animalType))
                    {
                        __result = __instance.type.Value.Contains(animalType, StringComparison.CurrentCultureIgnoreCase) && !building.isUnderConstruction();
                    }
                    else if (buildingData?.ValidOccupantTypes != null && buildingData.ValidOccupantTypes.Contains(__instance.buildingTypeILiveIn.Value) && !building.isUnderConstruction())
                    {
                        __result = building.GetIndoors() is AnimalHouse;
                    }
                }
                return false;
            }
        }

    }
}
