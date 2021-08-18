using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;

namespace AudioStudy.Bot.Domain.Services.Telegram
{
    public class TelegramMessagePipeline : ITelegramMessagePipeline
    {
        public Task HandleMessageAsync(TelegramRequestMessage requestMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}