using System;
using AudioStudy.Bot.Domain.Model.Telegram;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares.MenuSubMiddlewares
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MenuSubMiddlewareMetadataAttribute : Attribute
    {
        public TelegramState TelegramState { get; set; }
    }
}
