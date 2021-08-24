using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services.Telegram.Middlewares;
using AudioStudy.Bot.SharedUtils.Localization;
using AudioStudy.Bot.SharedUtils.Localization.Enums;
using Microsoft.Extensions.Logging;

namespace AudioStudy.Bot.Domain.Services.Telegram
{
    public class TelegramMessagePipeline : ITelegramMessagePipeline
    {
        private readonly ILogger<TelegramMessagePipeline> _logger;
        private readonly ITelegramClient _telegramClient;
        private readonly IBotLocalization _botLocalization;
        private readonly List<ITelegramPipelineMiddleware> _middlewares;
        
        public TelegramMessagePipeline(
            ILogger<TelegramMessagePipeline> logger,
            ITelegramClient telegramClient,
            IBotLocalization botLocalization,
            UserContextProviderMiddleware userContextProviderMiddleware,
            ChatTypeCheckerMiddleware chatTypeCheckerMiddleware,
            CommandExecutorMiddleware commandExecutorMiddleware,
            SettingsCheckerMiddleware settingsCheckerMiddleware,
            MenuMiddleware menuMiddleware,
            InlineKeyboardMiddleware inlineKeyboardMiddleware
            )
        {
            _logger = logger;
            _telegramClient = telegramClient;
            _botLocalization = botLocalization;
            _middlewares = new List<ITelegramPipelineMiddleware>
            {
                chatTypeCheckerMiddleware,
                userContextProviderMiddleware,
                settingsCheckerMiddleware,
                inlineKeyboardMiddleware,
                commandExecutorMiddleware,
                menuMiddleware
            };
        }
        public async Task HandleMessageAsync(TelegramRequestMessage requestMessage)
        {
            var context = new TelegramPipelineContext(requestMessage);
            try
            {
                foreach (var middleware in _middlewares)
                {
                    await middleware.HandleMessageAsync(context);
                    if (context.Processed)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Middleware error");
                await _telegramClient.SendAsync(new TelegramResponseMessage
                {
                    ChatId = requestMessage.ChatId,
                    Text = _botLocalization.UnexpectedErrorHasOccured(context.User?.Language ?? Language.Unknown)
                });
                return;
            }
            if (context.Processed && context.ResponseMessage != null)
            {
                context.ResponseMessage.ChatId = requestMessage.ChatId;
                try
                {
                    if (context.ResponseMessage.CallbackQueryId != null)
                    {
                        await _telegramClient.AnswerCallbackQuery(context.ResponseMessage.CallbackQueryId);
                    }
                    await _telegramClient.SendAsync(context.ResponseMessage);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Sending error");
                }
            }
        }
    }
}