using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model.Telegram;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares.MenuSubMiddlewares
{
    public interface IMenuSubMiddleware
    {
        Task ChangeState(TelegramPipelineContext pipelineContext, UserUpdateCommand updateCommand = null);
        Task ProcessState(TelegramPipelineContext pipelineContext);
    }
}
