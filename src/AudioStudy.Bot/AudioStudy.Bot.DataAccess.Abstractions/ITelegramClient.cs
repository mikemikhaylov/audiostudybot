using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;

namespace AudioStudy.Bot.DataAccess.Abstractions
{
    public interface ITelegramClient
    {
        Task<IReadOnlyList<TelegramUpdate>> GetUpdatesAsync(int offset, int limit, CancellationToken cancellationToken);
    }
}