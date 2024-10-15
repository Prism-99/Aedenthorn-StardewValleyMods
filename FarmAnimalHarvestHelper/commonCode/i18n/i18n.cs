using StardewModdingAPI;


namespace FarmAnimalHarvestHelper.I18n
{
    internal static class i18n
    {
        private static ITranslationHelper Translations;
        public static void Init(ITranslationHelper translations)
        {
            Translations = translations;
        }
        public static string ModEnabled()
        {
            return GetByKey("mod.enabled");
        }
        public static string MaxWait()
        {
            return GetByKey("max.wait");
        }
        public static string FirstSlot()
        {
            return GetByKey("first.slot");
        }
        public static string AnimalFix()
        {
            return GetByKey("animal.fix");
        }
        public static string AnimalFix_TT()
        {
            return GetByKey("animal.fix.tt");
        }
        public static string AnimalWarp()
        {
            return GetByKey("animal.warp");
        }
        public static string AnimalWarp_TT()
        {
            return GetByKey("animal.warp.tt");
        }
        public static string AnimalIndoorWarp()
        {
            return GetByKey("animal.indoor.warp");
        }
        public static string AnimalIndoorWarp_TT()
        {
            return GetByKey("animal.indoor.warp.tt");
        }
        public static string ExactAnimal()
        {
            return GetByKey("animal.exact");
        }
        public static string ExactAnimal_TT()
        {
            return GetByKey("animal.exact.tt");
        }
        public static string AnimalCycling()
        {
            return GetByKey("animal.cycling");
        }
        public static string AnimalCycling_TT()
        {
            return GetByKey("animal.cycling.tt");
        }
        public static Translation GetByKey(string key, object tokens = null)
        {
            if (Translations == null)
                throw new InvalidOperationException($"You must call {nameof(i18n)}.{nameof(Init)} from the mod's entry method before reading translations.");
            return Translations.Get(key, tokens);
        }
    }
}
