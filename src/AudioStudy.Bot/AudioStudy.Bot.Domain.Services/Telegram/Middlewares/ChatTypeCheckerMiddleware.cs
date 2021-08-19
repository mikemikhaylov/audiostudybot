using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.SharedUtils.Localization;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares
{
    public class ChatTypeCheckerMiddleware : ITelegramPipelineMiddleware
    {
        private readonly IBotLocalization _botLocalization;

        public ChatTypeCheckerMiddleware(IBotLocalization botLocalization)
        {
            _botLocalization = botLocalization;
        }
        
        public Task HandleMessageAsync(TelegramPipelineContext pipelineContext)
        {
            if (pipelineContext.RequestMessage.ChatType != TelegramChatType.Private)
            {
                pipelineContext.Processed = true;
                pipelineContext.ResponseMessage = new TelegramResponseMessage
                {
                    Text = _botLocalization.TelegramChatTypeIsNotSupported()
                };
            }
            return Task.CompletedTask;
        }
    }
}