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
        
        private string GetKey(Language language, string key) => _localizationSource.GetKey(language.GetMetadata().Short, key);
    }
}