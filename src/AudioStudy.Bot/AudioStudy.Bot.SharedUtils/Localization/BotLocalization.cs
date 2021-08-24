using System;
using System.Linq;
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

        public string ChooseLanguage() => FormatHelper.ConcatTelegramMessages(LanguageHelper.AllowedLanguages.Select(x => GetKey(x, "msg:chooselanguage")).ToArray());

        public string ChooseLanguage(Language language) => GetKey(language, "msg:chooselanguage");
        public string ChooseLearningLanguage(Language language) => GetKey(language, "msg:chooselearninglanguage");
        public string ChooseKnowsLanguage(Language language) => GetKey(language, "msg:chooseknowslanguage");
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

        public string HelpBtnLabel(Language language) => GetKey(language, "msg:helpbtn");
        public string LanguageBtnLabel(Language language)=> GetKey(language, "msg:languagebtn");

        public string CourseLanguage(Language language, string courseLanguage) => GetKey(language, $"msg:{courseLanguage}");
        public string DoYouLikeThisBotBtnLabel(Language language)=> FormatHelper.EmojiHeart + GetKey(language, "msg:ilikethisbotbtn");
        public string YourSettings(Language language)
        {
            return "<b>" + GetKey(language, "msg:currentsettings")
                         + ":</b>\n\n - " + $"{GetKey(language, "msg:language")}: <b>" + language.GetMetadata().Label +
                         "</b>";
        }

        public string NoCoursesOnCurrentPage(Language language) => "нет курсов на странице, запросите заново";

        public string PageBtnLabel(Language language) => "страница";

        public string NoCoursesMessage(Language language) => "в базе нет курсов попробуйте изменить фильтра";

        public string FilterCoursesBtn(Language language)=> "фильтр";

        public string CoursesMessage(Language language) => "вот это наши курсы";
        public string InlineBackBtn(Language language)=> "назад";

        private string GetKey(Language language, string key) => _localizationSource.GetKey((language == Language.Unknown ? Language.English : language).GetMetadata().Short, key);
    }
}