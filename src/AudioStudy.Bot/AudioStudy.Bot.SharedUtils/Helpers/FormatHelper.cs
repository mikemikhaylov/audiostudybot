using System.Collections.Generic;

namespace AudioStudy.Bot.SharedUtils.Helpers
{
    public static class FormatHelper
    {
        public const string TelegramNewLineSeparator = "\n";
        public const string TelegramConcatenatedMessagesSeparator = TelegramNewLineSeparator + TelegramNewLineSeparator;
        public const char TelegramCallbackDataDelimiter = '|';
        
        public const string EmojiSettings = "⚙️";
        public const string EmojiLanguage = "\U0001F310";
        public const string EmojiAddAsThumbsUp = "\U0001F44D";
        public const string EmojiPockerFace = "\U0001F610";
        public const string EmojiDissapointedFace = "\U0001F610";
        public const string EmojiConfusedFace = "\U0001F615";
        public const string EmojiSleepy = "\U0001F62A";
        public const string EmojiSmile = "\U0001F60A";
        public const string EmojiHeart = "\U00002764";

        public const string EmojiHelp = "ℹ️";
        public const string EmojiCourses = "📚️";
        public const string EmojiLearn = "🎧";
        public const string EmojiStar = "⭐";
        public const string EmojiFilter = "\U0001F32A";
        public const string CongratsEmoji = "🎉";
        public const string EmojiCheckMark = "✅";
        public const string EmojiStop = "⏹️";
        public const string EmojiStart = "▶️";
        public const string EmojiStartOver = "⏪";
        public const string EmojiCards = "🗃️";
        
        public const string EmojiAddAsCancel = "\U0001F519";

        public static string ConcatTelegramMessages(params string[] messages)
        {
            return string.Join(TelegramConcatenatedMessagesSeparator, messages);
        }

        public static string ConcatTelegramLines(params string[] messages)
        {
            return ConcatTelegramLines((IEnumerable<string>)messages);
        }

        public static string ConcatTelegramLines(IEnumerable<string> messages)
        {
            return string.Join(TelegramNewLineSeparator, messages);
        }
    }
}