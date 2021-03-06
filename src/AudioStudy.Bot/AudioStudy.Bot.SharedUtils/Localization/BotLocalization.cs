using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public string MainMenuText(Language language) => GetKey(language, "msg:mainmenutext");

        public string LessonNOfN(Language language, int numberOfLessons, int currentLesson)
        {
            return $"<b>{string.Format(GetKey(language, "msg:lessonnofn"), currentLesson, numberOfLessons)}</b>";
        }

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

        public string LearnBtnLabel(Language language) => FormatHelper.EmojiLearn + GetKey(language, "msg:learnbtn");

        public string CoursesBtnLabel(Language language) =>
            FormatHelper.EmojiCourses + GetKey(language, "msg:coursesbtn");

        public string HelpBtnLabel(Language language) => FormatHelper.EmojiHelp + GetKey(language, "msg:helpbtn");

        public string LanguageBtnLabel(Language language) =>
            FormatHelper.EmojiLanguage + GetKey(language, "msg:languagebtn");

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

        public string FilterCoursesBtn(Language language) =>
            FormatHelper.EmojiLanguage + GetKey(language, "msg:filter");

        public string CoursesMessage(Language language) => GetKey(language, "msg:listofcourses");

        public string InlineBackBtn(Language language) =>
            FormatHelper.EmojiAddAsCancel + GetKey(language, "msg:inlinebackbtn");

        public string Course(Language language, string courseName, string courseDescription, int numberOfCards,
            int numberOfLessons,
            bool isMyCourse, int lessonsLearned, bool isPhrasesCourse)
        {
            if (isMyCourse)
            {
                courseName = courseName + FormatHelper.EmojiStar;
            }

            var lines = new List<string> {$"<b>{courseName}</b>"};
            if (!string.IsNullOrWhiteSpace(courseDescription))
            {
                lines.Add(string.Empty);
                lines.Add(courseDescription);
            }

            lines.Add(string.Empty);
            var totalNumberOfCards = GetKey(language, isPhrasesCourse ? "msg:numberofphrases" : "msg:numberofwords");
            lines.Add($"<b>{totalNumberOfCards}:</b> {numberOfCards}");
            lines.Add(string.Empty);
            var lessonsLine = $"<b>{GetKey(language, "msg:numberoflessons")}:</b> {numberOfLessons}";
            if (isMyCourse)
            {
                if (lessonsLearned < 1)
                {
                    lessonsLearned = 0;
                }

                lessonsLine += $" ({GetKey(language, "msg:numberoflearnedlessons")} {lessonsLearned})";
                lines.Add(lessonsLine);
                if (lessonsLearned == numberOfLessons)
                {
                    lines.Add(string.Empty);
                    lines.Add($"{GetKey(language, "msg:coursecompleted")}" + FormatHelper.EmojiCheckMark);
                }
            }
            else
            {
                lines.Add(lessonsLine);
            }

            return FormatHelper.ConcatTelegramLines(lines);
        }

        public string CourseNotFound(Language language) => GetKey(language, "msg:coursenotfound");

        public string ShowCards(Language language, bool isPhrasesCourse)
        {
            return FormatHelper.EmojiCards + GetKey(language, isPhrasesCourse ? "msg:showphrases" : "msg:showwords");
        }

        public string StartCourseLearning(Language language)
        {
            return FormatHelper.EmojiStart + GetKey(language, "msg:startcourse");
        }

        public string StopCourseLearning(Language language)
        {
            return FormatHelper.EmojiStop + GetKey(language, "msg:stopcourse");
        }

        public string GetLesson(Language language)
        {
            return FormatHelper.EmojiLearn + GetKey(language, "msg:getlesson");
        }

        public string GetNextLesson(Language language)
        {
            return FormatHelper.EmojiLearn + GetKey(language, "msg:getnextlesson");
        }

        public string StartOverCourseLearning(Language language)
        {
            return FormatHelper.EmojiStartOver + GetKey(language, "msg:startover");
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
            return GetKey(language, "msg:finishcoursecongrats") + FormatHelper.CongratsEmoji;
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
            params (string Text, string Transcription, string Translation, string Usage, string UsageTranslation, bool
                isNew)[]
                cards)
        {
            var lines = new List<string>();
            for (int i = 0; i < cards.Length; i++)
            {
                var card = cards[i];
                var line = card.isNew ? FormatHelper.EmojiNew : string.Empty;
                line += HttpUtility.HtmlEncode(card.Text.Trim());
                if (!string.IsNullOrWhiteSpace(card.Transcription))
                {
                    line += HttpUtility.HtmlEncode($" [{card.Transcription.Trim()}]");
                }

                if (!string.IsNullOrWhiteSpace(card.Translation))
                {
                    line += $" - {HttpUtility.HtmlEncode(card.Translation.Trim())}";
                }

                lines.Add(line);
                if (!string.IsNullOrWhiteSpace(card.Usage))
                {
                    lines.Add(HttpUtility.HtmlEncode(card.Usage.Trim()));
                }

                if (!string.IsNullOrWhiteSpace(card.UsageTranslation))
                {
                    lines.Add(HttpUtility.HtmlEncode(card.UsageTranslation.Trim()));
                }

                if (i != cards.Length - 1)
                {
                    lines.Add(string.Empty);
                }
            }

            return FormatHelper.ConcatTelegramLines(lines);
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

        public string CurrentlyLearningCourse(Language language, (string name, int totalNumberOfLessons, int lessonsLearned) course)
        {
            return FormatHelper.ConcatTelegramLines(
                $"<b>{GetKey(language, "msg:course")}:</b> {course.name}",
                string.Empty,
                $"<b>{GetKey(language, "msg:numberoflessons")}:</b> {course.totalNumberOfLessons} ({GetKey(language, "msg:numberoflearnedlessons")} {course.lessonsLearned})"
                );
        }

        public string ChooseAnotherCourse(Language language)
        {
            return FormatHelper.EmojiCourses + GetKey(language, "msg:selectanothercourse");
        }

        public string CoursesToLearnMessage(Language language)
        {
            return string.Format(GetKey(language, "msg:choosecoursetolearn"), InlineCoursesBtnLabel(language));
        }

        public string NoCoursesToLearnMessage(Language language)
        {
            return GetKey(language, "msg:nocoursestolearn");
        }

        public string InlineCoursesBtnLabel(Language language)
        {
            return FormatHelper.EmojiCourses + GetKey(language, "msg:inlinecoursesbtn");
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
            return FormatHelper.EmojiConfusedFace + GetKey(language, "msg:iwillhideratebtn");
        }

        public string WillNotRateAnswer(Language language)
        {
            return FormatHelper.EmojiDissapointedFace + GetKey(language, "msg:thatsok");
        }

        public string UnknownCommand(Language language)
        {
            return FormatHelper.EmojiPockerFace + GetKey(language, "msg:unknowncommand");
        }

        private string GetKey(Language language, string key) =>
            _localizationSource.GetKey((language == Language.Unknown ? Language.English : language).GetMetadata().Short,
                key);
    }
}