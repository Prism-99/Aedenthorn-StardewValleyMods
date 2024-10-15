using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace FarmAnimalHarvestHelper
{
    internal static class Multiplayer
    {
        private static IManifest _manifest;
        private static IModHelper _modHelper;

        private const string msg_AnimalTended = "tendingMsg";
        private class HarvestHelper
        {
            public long animalID;
            public Guid buildingID;
            public string locationName = "";
            public override string ToString()
            {
                return $"Location: {locationName}, BuildingID: {buildingID}, AnimalID: {animalID}";
            }

        }
        public static void Init(IModHelper modHelper, IManifest manifest)
        {
            modHelper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;
            _manifest = manifest;
            _modHelper = modHelper;

        }
        public static void AnimalTended(FarmAnimal animal)
        {
            AnimalTended(animal.home.GetParentLocation().NameOrUniqueName, animal.home.id.Value, animal.myID.Value);
        }
        public static void AnimalTended(string location, Guid buildingID, long animalID)
        {
            HarvestHelper message = new HarvestHelper
            {
                locationName = location,
                buildingID = buildingID,
                animalID = animalID
            };

            _modHelper.Multiplayer.SendMessage(message, msg_AnimalTended, new[] { _manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            ModEntry.SMonitor.Log($"Tended animal msg sent. {message}", LogLevel.Debug);
        }
        /// <summary>
        /// Master game handle messages from farm hands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Message contents</param>
        private static void Multiplayer_ModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (Game1.IsMasterGame)
            {
                if (e.FromModID == _manifest.UniqueID)
                {
                    switch (e.Type)
                    {
                        case msg_AnimalTended:
                            HarvestHelper message = e.ReadAs<HarvestHelper>();
                            ModEntry.SMonitor.Log($"Tended animal msg recieved. {message}", LogLevel.Debug);
                            SlotManager.AnimalTended(message.locationName, message.buildingID, message.animalID);
                            break;
                    }
                }
            }
        }
    }
}
