namespace AudioStudy.Bot.Domain.Model.Telegram
{
    public enum TelegramInlineBtnType
    {
        OpenCourseListPage = 1,
        OpenCourse = 2,
        OpenCourseFilter = 3,
        OpenKnowsLanguageFilter = 4,
        SetCourseFilter = 5,
        StartLearning = 7,
        StopLearning = 8,
        StartOverFromCoursePage = 9,
        ConfirmStopLearning = 10,
        ConfirmStartOverFromCoursePage = 11,
        GetNextLesson = 12,
        StartOver = 13,
        OpenCourseCardsPage = 14,
        OpenLessonCardsPage = 15,
        OpenCourseListPageToStudy = 16,
        SetCourseToLearn = 17,
    }
}