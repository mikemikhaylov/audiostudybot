using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services.Telegram.Middlewares.MenuSubMiddlewares;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares
{
    public class MenuMiddleware : ITelegramPipelineMiddleware
    {
        private readonly IMenuSubMiddlewareFactory _menuSubMiddlewareFactory;

        public MenuMiddleware(IMenuSubMiddlewareFactory menuSubMiddlewareFactory)
        {
            _menuSubMiddlewareFactory = menuSubMiddlewareFactory;
        }
        public async Task HandleMessageAsync(TelegramPipelineContext pipelineContext)
        {
            var menuSubMiddleware = _menuSubMiddlewareFactory.Get(pipelineContext.User.State);
            await menuSubMiddleware.ProcessState(pipelineContext);
        }
    }
}