using AudioStudy.Bot.Domain.Model.Telegram;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares.MenuSubMiddlewares
{
    public interface IMenuSubMiddlewareFactory
    {
        IMenuSubMiddleware Get(TelegramState state);
    }
}