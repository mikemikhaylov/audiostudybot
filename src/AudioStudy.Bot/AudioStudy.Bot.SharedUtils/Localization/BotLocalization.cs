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

        public string ChooseLanguage() => FormatHelper.ConcatTelegramMessages(LanguageHelper.AllowedLanguages
            .Select(x => GetKey(x, "msg:chooselanguage")).ToArray());

        public string ChooseLanguage(Language language) => GetKey(language, "msg:chooselanguage");
        public string ChooseLearningLanguage(Language language) => GetKey(language, "msg:chooselearninglanguage");
        public string ChooseKnowsLanguage(Language language) => GetKey(language, "msg:chooseknowslanguage");
        public string TelegramChatTypeIsNotSupported() => "Chat type is not supported";

        public string UnexpectedErrorHasOccured(Language language) =>
            FormatHelper.EmojiPockerFace + GetKey(language, "msg:unexpectederror");

        public string Help(Language language) => GetKey(language, "msg:help");

        public string EveryThingSetUp(Language language) => string.Format(GetKey(language, "msg:everythingsetup"),
            FormatHelper.EmojiAddAsThumbsUp);

        public string CancelBtnLabel(Language language) =>
            FormatHelper.EmojiAddAsCancel + GetKey(language, "msg:cancelbtn");

        public string BackBtnLabel(Language language) =>
            FormatHelper.EmojiAddAsCancel + GetKey(language, "msg:backbtn");

        public string HaveRatedBtnLabel(Language language) =>
            FormatHelper.EmojiSmile + GetKey(language, "msg:ihaveratedbtn");

        public string WillRateLaterBtnLabel(Language language) =>
            FormatHelper.EmojiPockerFace + GetKey(language, "msg:willratelaterbtn");

        public string WillNotRateBtnLabel(Language language) =>
            FormatHelper.EmojiSleepy + GetKey(language, "msg:idontwanttoratebtn");

        public string SettingsBtnLabel(Language language) =>
            FormatHelper.EmojiSettings + GetKey(language, "msg:settingsbtn");

        public string LearnBtnLabel(Language language) => GetKey(language, "msg:learnbtn");

        public string CoursesBtnLabel(Language language) => GetKey(language, "msg:coursesbtn");

        public string HelpBtnLabel(Language language) => GetKey(language, "msg:helpbtn");
        public string LanguageBtnLabel(Language language) => GetKey(language, "msg:languagebtn");

        public string CourseLanguage(Language language, string courseLanguage) =>
            GetKey(language, $"msg:{courseLanguage}");

        public string DoYouLikeThisBotBtnLabel(Language language) =>
            FormatHelper.EmojiHeart + GetKey(language, "msg:ilikethisbotbtn");

        public string YourSettings(Language language)
        {
            return "<b>" + GetKey(language, "msg:currentsettings")
                         + ":</b>\n\n - " + $"{GetKey(language, "msg:language")}: <b>" + language.GetMetadata().Label +
                         "</b>";
        }

        public string NoCoursesOnCurrentPage(Language language) => GetKey(language, "msg:nocoursesonthepage");

        public string PageBtnLabel(Language language) => GetKey(language, "msg:page");

        public string NoCoursesMessage(Language language) => GetKey(language, "msg:nocourses");

        public string FilterCoursesBtn(Language language) => GetKey(language, "msg:filter") + FormatHelper.EmojiFilter;

        public string CoursesMessage(Language language) => GetKey(language, "msg:listofcourses");
        public string InlineBackBtn(Language language) => GetKey(language, "msg:inlinebackbtn");

        public string Course(Language language, string courseName, string courseDescription, int numberOfCards,
            int numberOfLessons,
            bool isMyCourse, int? lessonsLearned)
        {
            return string.Join(Environment.NewLine, $"Name {courseName}", $"isMyCourse {isMyCourse}",
                $"course completed {numberOfLessons <= lessonsLearned + 1}",
                $"Desc {courseDescription}", $"numberOfCards {numberOfCards}", $"lessons {numberOfLessons}",
                $"lessonsLearned {lessonsLearned}");
        }

        public string CourseNotFound(Language language) => "курс не найден";

        public string ShowCards(Language language)
        {
            return GetKey(language, "msg:showcards");
        }

        public string StartCourseLearning(Language language)
        {
            return GetKey(language, "msg:startcourse");
        }

        public string StopCourseLearning(Language language)
        {
            return GetKey(language, "msg:getnextlesson");
        }

        public string GetNextLesson(Language language)
        {
            return GetKey(language, "msg:getnextlesson");
        }

        public string StartOverCourseLearning(Language language)
        {
            return GetKey(language, "msg:startover");
        }

        public string AreYouSureStartOrStartOver(Language language)
        {
            return GetKey(language, "msg:suretostartoverorstop");
        }

        public string Yes(Language language)
        {
            return GetKey(language, "msg:yes");
        }

        public string No(Language language)
        {
            return GetKey(language, "msg:no");
        }

        public string YouFinishedTheCourse(Language language)
        {
            return GetKey(language, "msg:finishcoursecongrats");
        }

        public string ToTheCoursesList(Language language)
        {
            return GetKey(language, "msg:tothecourseslist");
        }

        public string CourseStartedOver(Language language)
        {
            return GetKey(language, "msg:coursestartedover");
        }

        public string NoCardsOnCurrentPage(Language language)
        {
            return GetKey(language, "msg:nocardsonthepage");
        }

        public string Cards(
            params (string Text, string Transcription, string Translation, string Usage, string UsageTranslation)[]
                cards)
        {
            return string.Join(Environment.NewLine, cards.Select(x => string.Join(Environment.NewLine, $"Text {x.Text}",
                $"Transcription {x.Transcription}",
                $"Translation {x.Translation}", $"Usage {x.Usage}", $"UsageTranslation {x.UsageTranslation}")));
        }

        public string NoCardsInCourse(Language language)
        {
            return GetKey(language, "msg:nocardsincourse");
        }

        public string NoCardsInLesson(Language language)
        {
            return GetKey(language, "msg:nocardsinlesson");
        }

        public string BackToTheCourse(Language language)
        {
            return InlineBackBtn(language);
        }

        public string CurrentlyLearningCourse(Language language, (string name, int totalNumberOfLessons) course)
        {
            return "вот этот курс сейчас " + course.name;
        }

        public string ChooseAnotherCourse(Language language)
        {
            return GetKey(language, "msg:selectanothercourse");
        }

        public string CoursesToLearnMessage(Language language)
        {
            return GetKey(language, "msg:choosecoursetolearn");
        }

        public string NoCoursesToLearnMessage(Language language)
        {
            return GetKey(language, "msg:nocoursestolearn");
        }

        public string InlineCoursesBtnLabel(Language language)
        {
            return GetKey(language, "msg:inlinecoursesbtn");
        }

        public string RatingInstruction(Language language)
        {
            return GetKey(language, "msg:ohgreate");
        }

        public string ThanksForRating(Language language)
        {
            return FormatHelper.EmojiSmile + GetKey(language, "msg:thankyou");
        }

        public string WillRateLaterAnswer(Language language)
        {
            return FormatHelper.EmojiSmile + GetKey(language, "msg:iwillhideratebtn");
        }

        public string WillNotRateAnswer(Language language)
        {
            return FormatHelper.EmojiSleepy + GetKey(language, "msg:thatsok");
        }

        public string UnknownCommand(Language language)
        {
            return GetKey(language, "msg:unknowncommand");
        }

        private string GetKey(Language language, string key) =>
            _localizationSource.GetKey((language == Language.Unknown ? Language.English : language).GetMetadata().Short,
                key);
    }
}