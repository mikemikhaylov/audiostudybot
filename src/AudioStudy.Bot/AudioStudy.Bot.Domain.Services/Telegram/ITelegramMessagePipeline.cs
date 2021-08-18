using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;

namespace AudioStudy.Bot.Domain.Services.Telegram
{
    public interface ITelegramMessagePipeline
    {
        Task HandleMessageAsync(TelegramRequestMessage requestMessage);
    }
}