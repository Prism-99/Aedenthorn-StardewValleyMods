using HarmonyLib;
using StardewValley.Tools;
using StardewValley;

namespace FarmAnimalHarvestHelper.Patches
{
    internal class ShearsPatches
    {
        //
        //  patch the Shears to capture shearing of the sheep to
        //  clear occupied slot
        //
        [HarmonyPatch(typeof(Shears), nameof(Shears.DoFunction))]
        public class Shears_DoFunction_Patch
        {
            /// <summary>
            /// Pass the animal as a state value for postfix to check if 
            /// the sheep was sheared
            /// </summary>
            public static bool Prefix(Shears __instance, out FarmAnimal? __state)
            {
                __state = __instance?.animal;
                return true;
            }
            public static void Postfix(Shears __instance, FarmAnimal? __state)
            {
                if (__state != null && __instance?.animal == null)
                {
                    SlotManager.AnimalTended(__state);                 
                }
            }
        }
    }
}
