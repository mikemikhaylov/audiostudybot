using AudioStudy.Bot.SharedUtils.Helpers;
using AudioStudy.Bot.SharedUtils.Localization.Enums;
using AudioStudy.Bot.SharedUtils.Localization.LocalizationSource;

namespace AudioStudy.Bot.SharedUtils.Localization
{
    public class BotLocalization : IBotLocalization
    {
        private readonly ILocalizationSource _localizationSource;

        public BotLocalization(ILocalizationSource localizationSource)
        {
            _localizationSource = localizationSource;
        }

        public string ChooseLanguage(Language language) => GetKey(language, "msg:chooselanguage");
        public string TelegramChatTypeIsNotSupported() => "Chat type is not supported";
        public string UnexpectedErrorHasOccured(Language language) => FormatHelper.EmojiPockerFace + GetKey(language, "msg:unexpectederror");
        public string Help(Language language) => GetKey(language, "msg:help");

        private string GetKey(Language language, string key) => _localizationSource.GetKey((language == Language.Unknown ? Language.English : language).GetMetadata().Short, key);
    }
}