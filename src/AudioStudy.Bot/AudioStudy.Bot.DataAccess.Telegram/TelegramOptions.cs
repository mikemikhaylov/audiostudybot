using System;

namespace AudioStudy.Bot.DataAccess.Telegram
{
    public class TelegramOptions
    {
        public readonly int TextMaxLength = 4000;
        public readonly int CaptionMaxLength = 1000;
        public string Token { get; init; }
        public TimeSpan PollingTimeout { get; init; } = TimeSpan.FromSeconds(10);
    }
}