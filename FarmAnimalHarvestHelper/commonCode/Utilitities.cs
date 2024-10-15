using StardewValley.Pathfinding;
using StardewValley;
using Microsoft.Xna.Framework;
using Rect = Microsoft.Xna.Framework.Rectangle;


namespace FarmAnimalHarvestHelper
{
    internal class Utilitities
    {
        public static void AnimalFinishedTending(FarmAnimal animal)
        {
            if (SlotManager.IsHoldingActive())
            {
                if (ModEntry.Config.EnableOutdoorAnimalWarp)
                {
                    if ((animal.home.animalDoorOpen.Value && !Game1.isRaining && !animal.home.GetParentLocation().IsSnowingHere()))
                    {
                        //
                        //  warp animal outside
                        //
#if !v168
                        AnimatedSprite orLoadTexture = animal.GetOrLoadTexture();
#endif
                        Rect rectForAnimalDoor = animal.home.getRectForAnimalDoor();
                        animal.home.GetParentLocation().animals.Remove(animal.myID.Value);
                        (animal.currentLocation as AnimalHouse)?.animals.Remove(animal.myID.Value);
                        animal.home.GetParentLocation().animals.Add(animal.myID.Value, animal);
                        animal.faceDirection(2);
                        animal.SetMovingDown(b: true);
#if !v168
                        animal.Position = new Vector2(rectForAnimalDoor.X, rectForAnimalDoor.Y - (orLoadTexture.getHeight() * 4 - animal.GetBoundingBox().Height) + 32);
#else
                    animal.Position = new Vector2(rectForAnimalDoor.X, rectForAnimalDoor.Y - (animal.Sprite.getHeight() * 4 - animal.GetBoundingBox().Height) + 32);
#endif
                        //
                        //  try to path the animal to grass
                        //
                        if (FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
                        {
                            FarmAnimal.NumPathfindingThisTick++;
                            animal.controller = new PathFindController(animal, animal.home.GetParentLocation(), FarmAnimal.grassEndPointFunction, Game1.random.Next(4), FarmAnimal.behaviorAfterFindingGrassPatch, 200, Point.Zero);
                        }
                        else
                        {
                            //
                            //  fallback place in random spot
                            //
                            animal.setRandomPosition(animal.home.GetIndoors());
                        }
                    }
                }
                else if (ModEntry.Config.EnableIndoorAnimalWarp)
                {
                    animal.setRandomPosition(animal.home.GetIndoors());
                }
            }
        }
    }
}
