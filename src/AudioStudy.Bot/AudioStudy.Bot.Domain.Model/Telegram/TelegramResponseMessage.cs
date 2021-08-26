using AudioStudy.Bot.SharedUtils.Helpers;

namespace AudioStudy.Bot.Domain.Model.Telegram
{
    public class TelegramResponseMessage
    {
        public long ChatId { get; set; }
        public string Text { get; set; }
        public bool Html { get; set; }
        public TelegramReplyBtn[][] ReplyButtons { get; set; }
        public TelegramInlineBtn[][] InlineButtons { get; set; }
        public string CallbackQueryId { get; set; }
        public int? CallbackMessageId { get; set; }
        public string FileId { get; set; }
        public bool IsCaption { get; set; }
    }
    
    public static class TelegramResponseMessageExtensions
    {
        public static TelegramResponseMessage AddText(this TelegramResponseMessage responseMessage, string text)
        {
            responseMessage ??= new TelegramResponseMessage();

            if (responseMessage.Text != null)
            {
                responseMessage.Text += FormatHelper.TelegramConcatenatedMessagesSeparator;
            }

            responseMessage.Text += text;
            return responseMessage;
        }
        
        public static TelegramResponseMessage SetReplyButtons(this TelegramResponseMessage responseMessage, TelegramReplyBtn[][] replyButtons)
        {
            responseMessage ??= new TelegramResponseMessage();
            responseMessage.ReplyButtons = replyButtons;
            return responseMessage;
        }
    }
}