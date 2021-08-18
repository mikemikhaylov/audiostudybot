namespace AudioStudy.Bot.Domain.Model.Telegram
{
    public class TelegramPipelineContext
    {
        public TelegramRequestMessage RequestMessage { get; }

        public TelegramPipelineContext(TelegramRequestMessage requestMessage)
        {
            RequestMessage = requestMessage;
        }

        public bool Processed { get; set; }
        public TelegramResponseMessage ResponseMessage { get; set; }
    }
}