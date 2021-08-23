using System;
using System.Linq;

namespace AudioStudy.Bot.SharedUtils.Localization.Enums
{
    public enum Language
    {
        [LanguageMetadata(Label = "Unknown", Short = "unknown")]
        Unknown = 0,

        [LanguageMetadata(Label = "English", Short = "en")]
        English = 1,

        [LanguageMetadata(Label = "Русский", Short = "ru")]
        Russian = 2
    }

    public static class LanguageHelper
    {
        private static Language[] GetLanguageEnumValues() => Enum.GetValues(typeof(Language))
            .Cast<Language>()
            .Where(x => x != Language.Unknown)
            .ToArray();

        public static Language[] AllowedLanguages => GetLanguageEnumValues();
    }
}