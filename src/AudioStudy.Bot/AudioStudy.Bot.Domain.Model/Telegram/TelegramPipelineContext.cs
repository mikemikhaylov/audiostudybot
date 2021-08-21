using AudioStudy.Bot.SharedUtils.Metrics;

namespace AudioStudy.Bot.Domain.Model.Telegram
{
    public class TelegramPipelineContext
    {
        public User User { get; set; }
        public TelegramRequestMessage RequestMessage { get; }

        public TelegramPipelineContext(TelegramRequestMessage requestMessage)
        {
            RequestMessage = requestMessage;
        }

        public bool Processed { get; set; }
        public TelegramResponseMessage ResponseMessage { get; set; }
        public Intent Intent { get; set; } = Intent.Unknown;
        public bool IntentNotHandled { get; set; }
    }
}