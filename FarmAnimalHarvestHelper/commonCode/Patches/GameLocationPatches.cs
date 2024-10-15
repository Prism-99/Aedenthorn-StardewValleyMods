using HarmonyLib;
using StardewValley.Buildings;
using StardewValley;


namespace FarmAnimalHarvestHelper.Patches
{
    internal class GameLocationPatches
    {
        /// <summary>
        /// Start of day scan to find animals to hold
        /// </summary>
        [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.updateEvenIfFarmerIsntHere))]
        private static class GameLocation_updateEvenIfFarmerIsntHere_Patch
        {
            public static void Postfix(GameLocation __instance)
            {
                if (Game1.IsMasterGame)
                {
                    int slotted = 0;
                    Building parent = VersionCode.GetParentBuilding(__instance);
                    if (__instance is AnimalHouse)
                    {
                        if (!SlotManager.slottedDict.ContainsKey(parent.GetIndoorsName()))
                        {
                            SlotManager.slottedDict.Add(parent.GetIndoorsName(), new());
                        }
                        else if (!ModEntry.Config.EnableAnimalCycling)
                            return;
                        // scan for animals that did not get slotted
                        //  only occurs is using barn enlargers
                        //
                        foreach (FarmAnimal animal in __instance.animals.Values)
                        {
                            if (SlotManager.slottedDict.TryGetValue(animal.home?.GetIndoorsName() ?? "", out var slots) && (!slots?.ContainsKey(animal.myID.Value) ?? false))
                            {
                                if (SlotManager.SlotAnimal(animal))
                                    slotted++;
                            }
                        }
                        return;
                    }
                    else if (__instance.IsOutdoors || !(parent?.buildingType.Value ?? "").Contains("Barn") || (parent?.GetData().MaxOccupants ?? 0) == 0 || __instance.uniqueName.Value == null || SlotManager.slottedDict.ContainsKey(__instance.uniqueName.Value) || __instance.animals.Count() == 0)
                        return;

                    if (!SlotManager.slottedDict.ContainsKey(__instance.uniqueName.Value))
                        SlotManager.slottedDict.Add(__instance.uniqueName.Value, new Dictionary<long, SlotManager.HoldingSpot>());

                    for (int i = __instance.animals.Count() - 1; i >= 0; i--)
                    {
                        if (SlotManager.SlotAnimal(__instance.animals.Pairs.ElementAt(i).Value))
                            slotted++;
                    }

                    ModEntry.SMonitor.Log($"Slotted {slotted} animails in {__instance.uniqueName.Value}");
                }
            }
        }
    }
}
