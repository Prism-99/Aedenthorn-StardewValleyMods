using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewModdingAPI;



namespace FarmAnimalHarvestHelper
{
    /// <summary>
    /// Manage holding slots
    /// </summary>
    public partial class SlotManager
    {
        public static readonly Dictionary<string, Dictionary<long, HoldingSpot>> slottedDict = new();
        public static readonly Dictionary<string, List<HoldingSpot>> buildingHoldingSpots = new();
        public class HoldingSpot
        {
            public Vector2 Position;
            public int Direction;
        }
        public static void RemoveAnimalHouse(string houseName)
        {
            slottedDict.Remove(houseName);
        }
        public static void AnimalTended(string location, Guid homeBuildingID, long animalID)
        {
            if (Game1.getLocationFromName(location) != null)
            {
                Building house = Game1.getLocationFromName(location).getBuildingById(homeBuildingID);
                if (house != null)
                {
                    if (house.GetIndoors().animals.TryGetValue(animalID, out FarmAnimal animal))
                    {
                        AnimalTended(animal);
                    }
                }
            }
        }
        public static void AnimalTended(FarmAnimal animal)
        {
            ModEntry.SMonitor.Log($"Animal tended. {Game1.IsMasterGame} ", LogLevel.Debug);
            if (Game1.IsMasterGame)
            {
                if (slottedDict.TryGetValue(animal.home.GetIndoorsName(), out Dictionary<long, HoldingSpot> dict))
                {
                    dict.Remove(animal.myID.Value);
                    Utilitities.AnimalFinishedTending(animal);
                }
            }
            else
            {
                Multiplayer.AnimalTended(animal);
            }
        }
        private static List<HoldingSpot> GetSlotList(AnimalHouse home)
        {
            List<HoldingSpot> slots = new();
            //
            //  check for Barn with custom holding spots
            //
            Building parent = VersionCode.GetParentBuilding(home);

            if (parent != null)
            {
                if (buildingHoldingSpots.TryGetValue(parent.buildingType.Value, out List<HoldingSpot> spots))
                    return spots;

                if (parent.modData.ContainsKey(ModEntry.modDataKey_customHoldingSpots))
                {
                    for (int i = 0; i < home.map.Layers[0].LayerWidth; i++)
                    {
                        for (int j = 0; j < home.map.Layers[0].LayerHeight; j++)
                        {
                            if (home.doesTileHaveProperty(i, j, "holdingspot", "Back") == null)
                            {
                                continue;
                            }

                            if (int.TryParse(home.GetTilePropertySplitBySpaces("holdingspot", "Back", i, j)[0], out int direction))
                            {
                                slots.Add(new HoldingSpot
                                {
                                    Position = new Vector2(i, j),
                                    Direction = direction
                                });
                            }
                        }
                    }
                    buildingHoldingSpots.Add(parent.buildingType.Value, slots);
                }
                else
                {
                    for (int i = 0; i < home.map.Layers[0].LayerWidth; i++)
                    {
                        for (int j = 0; j < home.map.Layers[0].LayerHeight; j++)
                        {
                            if (home.doesTileHaveProperty(i, j, "Trough", "Back") == null)
                            {
                                continue;
                            }

                            slots.Add(new HoldingSpot
                            {
                                Position = new Vector2(i - 0.5f, j + 2),
                                Direction = 0
                            });
                        }
                    }

                    buildingHoldingSpots.Add(parent.buildingType.Value, slots);
                }
            }
            return slots;
        }
        public static bool IsHoldingActive()
        {
            return ModEntry.Config.ModEnabled && ((ModEntry.Config.MaxWaitHour >= 0 && Game1.timeOfDay < ModEntry.Config.MaxWaitHour) || (ModEntry.Config.MaxWaitHour == 0 && Game1.timeOfDay > 600));
        }
        public static bool SlotAnimal(FarmAnimal animal)
        {
            if (!IsHoldingActive() || string.IsNullOrEmpty(animal.currentProduce.Value) || !slottedDict.ContainsKey(animal.home.GetIndoorsName()) || !animal.isAdult() || animal.type.Contains("Pig") || !animal.buildingTypeILiveIn.Contains("Barn"))
                return false;

            List<HoldingSpot> slots = GetSlotList(animal.home.GetIndoors() as AnimalHouse);

            for (int slot = 0; slot < slots.Count; slot++)
            {
                var occupied = slottedDict[animal.home.GetIndoorsName()].Values.Where(p => p.Position == slots[slot].Position);
                if (!occupied.Any())
                {
                    //
                    //  check to see if the spot is empty
                    //
                    //var currentAnimal = animal.home.GetIndoors().animals.Where(p => p.Values.Where(q => q.position.Value == new Vector2(pos.X * 64 - animal.GetAnimalData().SpriteHeight, pos.Y * 64)).Any());
                    //var currentAnimal = animal.home.GetIndoors().animals.Where(p => p.Values.Where(q => q.position.Value == new Vector2(slots[slot].X * 64 - animal.GetAnimalData().SpriteHeight, slots[slot].Y * 64)).Any());
                    //if (!currentAnimal.Any())
                    //{
                    slottedDict[animal.home.GetIndoorsName()][animal.myID.Value] = slots[slot];
                    WarpAnimalToHoldingSpot(slots[slot], animal);
                    //if (animal.currentLocation == null || animal.currentLocation.NameOrUniqueName != animal.home.GetIndoorsName())
                    //{
                    //    if (animal.currentLocation != null)
                    //        animal.currentLocation.animals.Remove(animal.myID.Value);
                    //    if (!animal.home.GetIndoors().animals.ContainsKey(animal.myID.Value))
                    //        animal.home.GetIndoors().animals.Add(animal.myID.Value, animal);
                    //    animal.currentLocation = animal.home.GetIndoors();
                    //}
                    return true;
                    //}
                }
            }

            return false;
        }
        public static void WarpAnimalToHoldingSpot(HoldingSpot? spot, FarmAnimal animal)
        {
            if (spot != null)
            {
                //if (animal.currentLocation == null || animal.currentLocation.NameOrUniqueName != animal.home.GetIndoorsName())
                //{
                if (animal.currentLocation != null && animal.currentLocation.NameOrUniqueName != animal.home.GetIndoorsName())
                    animal.currentLocation.animals.Remove(animal.myID.Value);
                if (!animal.home.GetIndoors().animals.ContainsKey(animal.myID.Value))
                    animal.home.GetIndoors().animals.Add(animal.myID.Value, animal);
                animal.currentLocation = animal.home.GetIndoors();
                animal.Position = new Vector2((spot.Position.X) * 64, (spot.Position.Y - 1) * 64);
                animal.faceDirection(spot.Direction);
                //}
            }
        }
        private static int GetHoldingSpotsCount(AnimalHouse house)
        {
            string buildingType = VersionCode.GetParentBuilding(house).buildingType.Value;

            if (buildingHoldingSpots.TryGetValue(buildingType, out List<HoldingSpot> spots))
                return spots.Count;

            return GetSlotList(house).Count;
        }
        public static bool TryGetHoldVector(FarmAnimal animal, GameLocation location, out HoldingSpot? holdingSpot)
        {
            holdingSpot = null;
            //if (!IsHoldingActive() || location is not AnimalHouse || string.IsNullOrEmpty(animal.currentProduce.Value) || !animal.isAdult() || animal.type.Contains("Pig") || !slottedDict.TryGetValue(animal.home.GetIndoorsName(), out Dictionary<long, holdingSpot> dict) || !dict.TryGetValue(animal.myID.Value, out milkingSpot))
            if (!IsHoldingActive() || string.IsNullOrEmpty(animal.currentProduce.Value) || !animal.isAdult() || animal.type.Contains("Pig") || !slottedDict.TryGetValue(animal.home?.GetIndoorsName() ?? "", out Dictionary<long, HoldingSpot> dict) || !dict.TryGetValue(animal.myID.Value, out holdingSpot))
                return false;

            return true;
        }

    }
}
