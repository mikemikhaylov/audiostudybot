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

        public async Task<IReadOnlyList<TelegramUpdate>> GetUpdatesAsync(int offset, int limit,
            CancellationToken cancellationToken)
        {
            var result =
                await _telegramBotClient.GetUpdatesAsync(offset, limit, _config.PollingTimeout.Seconds, new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                }, cancellationToken);
            return result.Select(x => new TelegramUpdate
            {
                Id = x.Id,
                Message = x.Message == null
                    ? null
                    : new TelegramMessage
                    {
                        Text = x.Message.Text
                    }
            }).ToList();
        }
    }
}