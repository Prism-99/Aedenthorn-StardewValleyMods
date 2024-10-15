using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealNamesREDUX.i18n
{
    internal class i18n
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
        public static string GenderNeutral()
        {
            return GetByKey("neutralname");
        }
        public static string GenderNeutralDesc()
        {
            return GetByKey("neutralname.desc");
        }
        public static string LocaleString()
        {
            return GetByKey("localestring");
        }
      
        public static string LocaleStringDesc()
        {
            return GetByKey("localestring.desc");
        }
        public static string AnimalNames()
        {
            return GetByKey("realanimalnames");
        }
        public static string AnimalNamesDesc()
        {
            return GetByKey("realanimalnames.desc");
        }
        private static Translation GetByKey(string key, object tokens = null)
        {
            if (Translations == null)
                throw new InvalidOperationException($"You must call {nameof(i18n)}.{nameof(Init)} from the mod's entry method before reading translations.");
            return Translations.Get(key, tokens);
        }
    }
}
