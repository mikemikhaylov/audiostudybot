using AudioStudy.Bot.SharedUtils.Localization.Enums;

namespace AudioStudy.Bot.SharedUtils.Localization
{
    public interface IBotLocalization
    {
        string ChooseLanguage();
        string ChooseLanguage(Language language);
        string ChooseLearningLanguage(Language language);
        string ChooseKnowsLanguage(Language language);
        string TelegramChatTypeIsNotSupported();
        string UnexpectedErrorHasOccured(Language language);
        string Help(Language language);
        string CancelBtnLabel(Language language);
        string BackBtnLabel(Language language);
        string HaveRatedBtnLabel(Language language);
        string WillRateLaterBtnLabel(Language language);
        string WillNotRateBtnLabel(Language language);
        string SettingsBtnLabel(Language language);
        string LearnBtnLabel(Language language);
        string CoursesBtnLabel(Language language);
        string HelpBtnLabel(Language language);
        string LanguageBtnLabel(Language language);
        string CourseLanguage(Language language, string courseLanguage);
        string DoYouLikeThisBotBtnLabel(Language language);
        string YourSettings(Language language);
        string NoCoursesOnCurrentPage(Language language);
        string PageBtnLabel(Language language);
        string NoCoursesMessage(Language language);
        string FilterCoursesBtn(Language language);
        string CoursesMessage(Language language);
        string InlineBackBtn(Language language);

        string Course(Language language, string courseName, string courseDescription, int numberOfCards,
            int numberOfLessons, bool isMyCourse, int? lessonsLearned);

        string CourseNotFound(Language language);
        string ShowCards(Language language);
        string StartCourseLearning(Language language);
        string StopCourseLearning(Language language);
        string GetNextLesson(Language language);
        string StartOverCourseLearning(Language language);
        string AreYouSureStartOrStartOver(Language language);
        string Yes(Language language);
        string No(Language language);
        string YouFinishedTheCourse(Language language);
        string ToTheCoursesList(Language language);
        string CourseStartedOver(Language language);
        string HereIsYourLesson(Language language);
        string NoCardsOnCurrentPage(Language language);
        string Cards(params (string Text, string Transcription, string Translation, string Usage, string UsageTranslation)[] cards);
        string NoCardsInCourse(Language language);
        string NoCardsInLesson(Language language);
        string BackToTheCourse(Language language);
        string CurrentlyLearningCourse(Language language, (string name, int totalNumberOfLessons) course);
        string ChooseAnotherCourse(Language language);
        string CoursesToLearnMessage(Language language);
        string NoCoursesToLearnMessage(Language language);
        string InlineCoursesBtnLabel(Language language);
        string RatingInstruction(Language language);
        string ThanksForRating(Language language);
        string WillRateLaterAnswer(Language language);
        string WillNotRateAnswer(Language language);
        string UnknownCommand(Language language);
    }
}