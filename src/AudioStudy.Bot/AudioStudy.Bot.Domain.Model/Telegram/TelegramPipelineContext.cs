namespace AudioStudy.Bot.Domain.Model.Telegram
{
    public class TelegramPipelineContext
    {
        public User User { get; }
        public TelegramRequestMessage RequestMessage { get; }

        public TelegramPipelineContext(TelegramRequestMessage requestMessage, User user)
        {
            RequestMessage = requestMessage;
            User = user;
        }

        public bool Processed { get; set; }
        public TelegramResponseMessage ResponseMessage { get; set; }
    }
}