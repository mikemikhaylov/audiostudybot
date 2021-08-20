using System.Collections.Generic;

namespace AudioStudy.Bot.SharedUtils.Helpers
{
    public static class FormatHelper
    {
        public const string TelegramNewLineSeparator = "\n";
        public const string TelegramConcatenatedMessagesSeparator = TelegramNewLineSeparator + TelegramNewLineSeparator;
        public const char TelegramCallbackDataDelimiter = '|';

        public const string EmojiPlus = "\U00002795";
        public const string EmojiList = "\U0001F4C4";
        public const string EmojiSettings = "\U0001F527";
        public const string EmojiLanguage = "\U0001F310";
        public const string EmojiAddAsThumbsUp = "\U0001F44D";
        public const string EmojiKey = "\U0001F511";
        public const string EmojiAddAsClapping = "\U0001F44F";
        public const string EmojiFoldedHands = "\U0001F64F";
        public const string EmojiCake = "\U0001F382";
        public const string EmojiScroll = "\U0001F4DC";
        public const string EmojiNoEntry = "\U000026D4";
        public const string EmojiCheckBox = "\U00002705";
        public const string EmojiPockerFace = "\U0001F610";
        public const string EmojiDissapointedFace = "\U0001F610";
        public const string EmojiConfusedFace = "\U0001F615";
        public const string EmojiHourglass = "\U000023F3";
        public const string EmojiCrossedBell = "\U0001F507";
        public const string EmojiSleepy = "\U0001F62A";
        public const string EmojiSmile = "\U0001F60A";
        public const string EmojiSmile2 = "\U0001F642";
        public const string EmojiHeart = "\U00002764";
        public const string EmojiPage = "\U0001F4C4";
        public const string EmojiOk = "\U0001F44C";
        public const string EmojiPointingFingerDown = "\U0001F447";
        public const string EmojiSquare = "\U000025FB";
        public const string EmojiCheckedSquare = "\U00002611";

        public const string EmojiOff = "\U0001F4F4";
        public const string EmojiOn = "\U0001F51B";
        public const string EmojiFingerUp = "\U0000261D";

        public const string EmojiCheckboxChecked = "\U00002611";
        public const string EmojiCheckboxUnchecked = "\U000025FD";
        
        public const string EmojiAddAsCancel = "\U0001F519";

        public static string FormatTelegramReminderText(string reminderText)
        {
            return $"\U0001F4E2 {reminderText}";
        }

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