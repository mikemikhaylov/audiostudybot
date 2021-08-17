namespace AudioStudy.Bot.Domain.Model.Telegram
{
    public class TelegramUpdate
    {
        public int Id { get; init; }
        public TelegramMessage Message { get; init; }
    }
}