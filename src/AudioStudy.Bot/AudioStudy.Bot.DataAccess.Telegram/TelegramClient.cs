using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model.Telegram;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace AudioStudy.Bot.DataAccess.Telegram
{
    public class TelegramClient : ITelegramClient
    {
        private readonly TelegramOptions _config;
        private readonly TelegramBotClient _telegramBotClient;

        public TelegramClient(IOptions<TelegramOptions> config)
        {
            _config = config.Value;
            _telegramBotClient = new TelegramBotClient(_config.Token);
        }

        public async Task<IReadOnlyList<TelegramRequestMessage>> GetUpdatesAsync(int offset, int limit,
            CancellationToken cancellationToken)
        {
            var result =
                await _telegramBotClient.GetUpdatesAsync(offset, limit, _config.PollingTimeout.Seconds, new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                }, cancellationToken);
            return result.Select(x =>
            {
                var chat = x.Message?.Chat ?? x.CallbackQuery?.Message?.Chat;
                return new TelegramRequestMessage
                {
                    Id = x.Id,
                    Text = x.Message?.Text,
                    ChatId = chat?.Id ?? default,
                    ChatType = GetTelegramChatType(chat?.Type),
                    CallbackMessageId = x.CallbackQuery?.Message?.MessageId,
                    CallbackQueryId = x.CallbackQuery?.Id,
                    CallbackData = x.CallbackQuery?.Data
                };
            }).ToList();
        }
        
        private static TelegramChatType GetTelegramChatType(ChatType? chatType)
        {
            return chatType switch
            {
                ChatType.Channel => TelegramChatType.Channel,
                ChatType.Group => TelegramChatType.Group,
                ChatType.Private => TelegramChatType.Private,
                ChatType.Supergroup => TelegramChatType.SuperGroup,
                _ => TelegramChatType.Unknown
            };
        }
    }
}