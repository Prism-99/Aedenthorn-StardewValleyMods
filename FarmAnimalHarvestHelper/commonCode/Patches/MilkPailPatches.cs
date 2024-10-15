using HarmonyLib;
using StardewValley.Tools;
using StardewValley;

namespace FarmAnimalHarvestHelper.Patches
{
    internal class MilkPailPatches
    {
        //
        //  patch the MilkPail to capture milking of the cow/goat to
        //  clear occupied slot
        //
        [HarmonyPatch(typeof(MilkPail), nameof(MilkPail.DoFunction))]
        public class MilkPail_DoFunction_Patch
        {
            /// <summary>
            /// Pass the animal as a state value for postfix to check if 
            /// the animal was milked
            /// </summary>
            public static bool Prefix(MilkPail __instance, out FarmAnimal? __state)
            {
                __state = __instance?.animal;
                return true;
            }
            public static void Postfix(MilkPail __instance, FarmAnimal? __state)
            {
                if (__state != null && __instance?.animal == null)
                {
                    SlotManager.AnimalTended(__state);
                }

            }
        }
    }
}
