namespace AudioStudy.Bot.Domain.Model.Telegram
{
    public class TelegramResponseMessage
    {
        public long ChatId { get; set; }
        public string Text { get; set; }
        public bool Html { get; set; }
    }
}