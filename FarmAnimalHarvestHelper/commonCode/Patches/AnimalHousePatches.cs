using StardewValley;
using HarmonyLib;

namespace FarmAnimalHarvestHelper.Patches
{
    /// <summary>
    /// Clear slot records at the start of the day
    /// </summary>
    [HarmonyPatch(typeof(AnimalHouse), nameof(AnimalHouse.DayUpdate))]
    internal class AnimalHousePatches
    {
        public class AnimalHouse_DayUpdate_Patch
        {
            public static void Postfix(AnimalHouse __instance)
            {
                if (Game1.IsMasterGame)
                {
                    SlotManager.RemoveAnimalHouse(__instance.uniqueName.Value);
                }
            }
        }
    }
}
