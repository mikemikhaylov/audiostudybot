using AudioStudy.Bot.SharedUtils.Localization.Enums;

namespace AudioStudy.Bot.SharedUtils.Localization
{
    public interface IBotLocalization
    {
        string ChooseLanguage(Language language);
        string TelegramChatTypeIsNotSupported();
        string UnexpectedErrorHasOccured(Language language);
        string Help(Language language);
    }
}