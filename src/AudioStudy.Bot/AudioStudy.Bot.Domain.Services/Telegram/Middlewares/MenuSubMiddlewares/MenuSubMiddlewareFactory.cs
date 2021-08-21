using System;
using System.Collections.Generic;
using AudioStudy.Bot.Domain.Model.Telegram;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares.MenuSubMiddlewares
{
    public class MenuSubMiddlewareFactory : IMenuSubMiddlewareFactory
    {
        private readonly Dictionary<TelegramState, IMenuSubMiddleware> _subMiddlewareByState = new();

        public MenuSubMiddlewareFactory(IEnumerable<IMenuSubMiddleware> subMiddlewares)
        {
            foreach (var subMiddleware in subMiddlewares)
            {
                MenuSubMiddlewareMetadataAttribute metaData =
                    (MenuSubMiddlewareMetadataAttribute)
                    Attribute.GetCustomAttribute(subMiddleware.GetType(), typeof(MenuSubMiddlewareMetadataAttribute));
                if (metaData == null)
                {
                    throw new Exception($"{subMiddleware.GetType().Name} does not contain metadata");
                }
                if (_subMiddlewareByState.ContainsKey(metaData.TelegramState))
                {
                    throw new Exception($"More than 1 {nameof(IMenuSubMiddleware)} for state {metaData.TelegramState} has been found.");
                }
                _subMiddlewareByState[metaData.TelegramState] = subMiddleware;
            }
        }

        public IMenuSubMiddleware Get(TelegramState state)
        {
            if (_subMiddlewareByState.TryGetValue(state, out var subMiddlewareType))
            {
                return subMiddlewareType;
            }
            throw new Exception($"{nameof(IMenuSubMiddleware)} for state {state} is not implemented");
        }
    }
}
