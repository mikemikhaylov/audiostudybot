using AudioStudy.Bot.SharedUtils.Helpers;
using AudioStudy.Bot.SharedUtils.Localization.Enums;
using AudioStudy.Bot.SharedUtils.Localization.LocalizationSource;

namespace AudioStudy.Bot.SharedUtils.Localization
{
    public class BotLocalization : IBotLocalization
    {
        private readonly ILocalizationSource _localizationSource;

        public BotLocalization(ILocalizationSource localizationSource)
        {
            _localizationSource = localizationSource;
        }

        public string ChooseLanguage(Language language) => GetKey(language, "msg:chooselanguage");
        public string TelegramChatTypeIsNotSupported() => "Chat type is not supported";
        public string UnexpectedErrorHasOccured(Language language) => FormatHelper.EmojiPockerFace + GetKey(language, "msg:unexpectederror");
        public string Help(Language language) => GetKey(language, "msg:help");
        public string CancelBtnLabel(Language language) => FormatHelper.EmojiAddAsCancel + GetKey(language, "msg:cancelbtn");

        public string BackBtnLabel(Language language) => FormatHelper.EmojiAddAsCancel + GetKey(language, "msg:backbtn");

        public string HaveRatedBtnLabel(Language language) => FormatHelper.EmojiSmile + GetKey(language, "msg:ihaveratedbtn");

        public string WillRateLaterBtnLabel(Language language) => FormatHelper.EmojiPockerFace + GetKey(language, "msg:willratelaterbtn");

        public string WillNotRateBtnLabel(Language language) => FormatHelper.EmojiSleepy + GetKey(language, "msg:idontwanttoratebtn");

        public string SettingsBtnLabel(Language language) => FormatHelper.EmojiSettings + GetKey(language, "msg:settingsbtn");

        public string LearnBtnLabel(Language language) => GetKey(language, "msg:learnbtn");

        public string CoursesBtnLabel(Language language) => GetKey(language, "msg:coursesbtn");

        public string MyCoursesBtnLabel(Language language) => GetKey(language, "msg:mycoursesbtn");

        public string HelpBtnLabel(Language language) => GetKey(language, "msg:helpbtn");

        public string CourseLanguage(Language language, string courseLanguage) => GetKey(language, "msg:help");

        private string GetKey(Language language, string key) => _localizationSource.GetKey((language == Language.Unknown ? Language.English : language).GetMetadata().Short, key);
    }
}