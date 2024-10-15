using Microsoft.Xna.Framework;


namespace FarmAnimalHarvestHelper
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public int MaxWaitHour { get; set; } = 1200;
        public Vector2 FirstSlotTile { get; set; } = new Vector2(8, 4);
        public bool EnableFarmAnimalFix { get; set; } = false;
        public bool EnableExactAnimals { get; set; } = false;
        public bool EnableOutdoorAnimalWarp { get; set; } = false;
        public bool EnableIndoorAnimalWarp { get; set; } = false;
        public bool EnableAnimalCycling { get; set; } = false;
    }
}
