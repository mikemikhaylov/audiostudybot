using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares
{
    public interface ITelegramPipelineMiddleware
    {
        Task HandleMessageAsync(TelegramPipelineContext pipelineContext);
    }
}
