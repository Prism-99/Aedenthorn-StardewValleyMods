using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using FarmAnimalHarvestHelper.I18n;
using StardewModdingAPI.Events;

namespace FarmAnimalHarvestHelper
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {

        private static IMonitor _Monitor;
        public static ModConfig Config;
        private static ModEntry context;
        public static bool firstPass;
        public const string modDataKey_customHoldingSpots = "prism99.fahh.holdingspots";
        public const string modDataKey_useExactHouseRules = "prism99.fahh.animaltype";
        public const string slottedKey = "aedenthorn.FarmAnimalHarvestHelper/slotted";

        private Harmony harmony;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            i18n.Init(helper.Translation);

            context = this;

            _Monitor = Monitor;

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();

            Multiplayer.Init(helper, ModManifest);
        }
        public static IMonitor SMonitor => _Monitor;
        private void GameLoop_DayStarted(object? sender, DayStartedEventArgs e)
        {
            SlotManager.slottedDict.Clear();
        }

        private void GameLoop_SaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            SlotManager.slottedDict.Clear();
        }

        private void GameLoop_GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => i18n.ModEnabled(),
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => i18n.MaxWait(),
                getValue: () => Config.MaxWaitHour,
                setValue: value => Config.MaxWaitHour = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => i18n.FirstSlot(),
                getValue: () => Config.FirstSlotTile.X + "," + Config.FirstSlotTile.Y,
                setValue: delegate (string value)
                {
                    string[] split = value.Split(',');
                    if (split.Length == 2 && int.TryParse(split[0], out int x) && int.TryParse(split[1], out int y))
                    {
                        Config.FirstSlotTile = new Vector2(x, y);
                    }
                }
            );

            configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => i18n.AnimalWarp(),
            tooltip: () => i18n.AnimalWarp_TT(),
            getValue: () => Config.EnableOutdoorAnimalWarp,
            setValue: value => Config.EnableOutdoorAnimalWarp = value
        );
            configMenu.AddBoolOption(
           mod: ModManifest,
           name: () => i18n.AnimalIndoorWarp(),
           tooltip: () => i18n.AnimalIndoorWarp_TT(),
           getValue: () => Config.EnableIndoorAnimalWarp,
           setValue: value => Config.EnableIndoorAnimalWarp = value
       );
            configMenu.AddBoolOption(
          mod: ModManifest,
          name: () => i18n.AnimalCycling(),
          tooltip: () => i18n.AnimalCycling_TT(),
          getValue: () => Config.EnableAnimalCycling,
          setValue: value => Config.EnableAnimalCycling = value
      );
            configMenu.AddBoolOption(
               mod: ModManifest,
               name: () => i18n.AnimalFix(),
               tooltip: ()=> i18n.AnimalFix_TT(),
               getValue: () => Config.EnableFarmAnimalFix,
               setValue: value => Config.EnableFarmAnimalFix = value
           );

            configMenu.AddBoolOption(
           mod: ModManifest,
           name: () => i18n.ExactAnimal(),
           tooltip: () => i18n.ExactAnimal_TT(),
           getValue: () => Config.EnableExactAnimals,
           setValue: value => Config.EnableExactAnimals = value
       );
        }
    }
}