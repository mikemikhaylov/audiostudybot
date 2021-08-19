namespace AudioStudy.Bot.Domain.Model.Telegram
{
    public enum TelegramState
    {
        Unknown = 0,
        OnMainWindow = 1,
        AwaitingForLanguage = 2,
        OnSettingsWindow = 3,
        OnRatingWindow = 4
    }
}