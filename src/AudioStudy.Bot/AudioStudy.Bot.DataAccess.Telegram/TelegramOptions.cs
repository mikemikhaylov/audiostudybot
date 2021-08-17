using System;

namespace AudioStudy.Bot.DataAccess.Telegram
{
    public class TelegramOptions
    {
        public string Token { get; init; }
        public TimeSpan PollingTimeout { get; init; } = TimeSpan.FromSeconds(10);
    }
}