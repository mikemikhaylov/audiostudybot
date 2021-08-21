using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.SharedUtils.Localization.Enums;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public interface ITelegramButtonsHelper
    {
        bool TryGetLang(string text, out Language language);
        bool TryCourseLang(string text, out string language);
        TelegramReplyBtn[][] ForceLearningLanguageButtons(Language language);
        TelegramReplyBtn[][] ForceKnowsLanguageButtons(Language language, string learningLanguage);
        TelegramReplyBtn[][] ForceLanguageButtons { get; }
        TelegramReplyBtn[][] GetStateButtons(TelegramState telegramState, User user);
    }
}