namespace AudioStudy.Bot.Domain.Model.Telegram
{
    public enum TelegramState
    {
        Unknown = 0,
        OnMainWindow = 1,
        AwaitingForLanguage = 2,
        AwaitingLearningLanguage = 3,
        AwaitingKnowsLanguage = 4,
        OnSettingsWindow = 5,
        OnRatingWindow = 6
    }
}