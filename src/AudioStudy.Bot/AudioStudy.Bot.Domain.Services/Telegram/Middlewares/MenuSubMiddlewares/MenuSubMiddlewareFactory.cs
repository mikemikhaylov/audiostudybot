using System;
using System.Collections.Generic;
using System.Linq;
using AudioStudy.Bot.Domain.Model.Telegram;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares.MenuSubMiddlewares
{
    public class MenuSubMiddlewareFactory : IMenuSubMiddlewareFactory
    {
        private readonly Lazy<Dictionary<TelegramState, IMenuSubMiddleware>> _subMiddlewareByState;

        public MenuSubMiddlewareFactory(Func<IEnumerable<IMenuSubMiddleware>> subMiddlewares)
        {
            _subMiddlewareByState = new Lazy<Dictionary<TelegramState, IMenuSubMiddleware>>(() =>
            {
                Dictionary<TelegramState, IMenuSubMiddleware> subMiddlewareByState = new();
                foreach (var subMiddleware in subMiddlewares())
                {
                    MenuSubMiddlewareMetadataAttribute metaData =
                        (MenuSubMiddlewareMetadataAttribute)
                        Attribute.GetCustomAttribute(subMiddleware.GetType(), typeof(MenuSubMiddlewareMetadataAttribute));
                    if (metaData == null)
                    {
                        throw new Exception($"{subMiddleware.GetType().Name} does not contain metadata");
                    }
                    if (subMiddlewareByState.ContainsKey(metaData.TelegramState))
                    {
                        throw new Exception($"More than 1 {nameof(IMenuSubMiddleware)} for state {metaData.TelegramState} has been found.");
                    }
                    subMiddlewareByState[metaData.TelegramState] = subMiddleware;
                }

                return subMiddlewareByState;
            });
            
        }

        public IMenuSubMiddleware Get(TelegramState state)
        {
            if (_subMiddlewareByState.Value.TryGetValue(state, out var subMiddlewareType))
            {
                return subMiddlewareType;
            }
            throw new Exception($"{nameof(IMenuSubMiddleware)} for state {state} is not implemented");
        }
    }
}
