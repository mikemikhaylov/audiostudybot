namespace AudioStudy.Bot.Domain.Model.Telegram
{
    public class TelegramRequestMessage
    {
        public int Id { get; init; }
        public string Text { get; init; }
        public long ChatId { get; set; }
        public TelegramChatType ChatType { get; set; }
        public int? CallbackMessageId { get; set; }
        public string CallbackQueryId { get; set; }
        public string CallbackData { get; set; }
    }
}