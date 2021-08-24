namespace AudioStudy.Bot.Domain.Model.Telegram
{
    public class TelegramInlineBtn
    {
        public string Text { get; set; }
        public string CallbackData { get; set; }

        public TelegramInlineBtn(string text, string callbackData)
        {
            Text = text;
            CallbackData = callbackData;
        }
    }
}