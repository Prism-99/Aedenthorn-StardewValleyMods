using StardewModdingAPI;

namespace CustomPictureFrames
{
    internal static class i18n
    {
        private static ITranslationHelper Translations;
        public static void Init(ITranslationHelper translations)
        {
            Translations = translations;
        }
        public static string Enabled()
        {
            return GetByKey("enabled");
        }
        public static string StartFraming()
        {
            return GetByKey("startframing");
        }
        public static string SwitchFrame()
        {
            return GetByKey("switchframe");
        }
        public static string TakePicture()
        {
            return GetByKey("takepicture");
        }
        public static string SwitchPicture()
        {
            return GetByKey("switchpicture");
        }
        public static string DeletePicture()
        {
            return GetByKey("deletepicture");
        }
        private static Translation GetByKey(string key, object tokens = null)
        {
            if (Translations == null)
                throw new InvalidOperationException($"You must call {nameof(i18n)}.{nameof(Init)} from the mod's entry method before reading translations.");
            return Translations.Get(key, tokens);
        }
    }
}
