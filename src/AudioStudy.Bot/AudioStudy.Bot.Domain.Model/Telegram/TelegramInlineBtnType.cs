namespace AudioStudy.Bot.Domain.Model.Telegram
{
    public enum TelegramInlineBtnType
    {
        OpenCourseListPage = 1,
        OpenCourse = 2,
        OpenCourseFilter = 3,
        OpenKnowsLanguageFilter = 4,
        SetCourseFilter = 5,
        ShowCards = 6,
        StartLearning = 7,
        StopLearning = 8,
        StartOverFromCoursePage = 9,
        ConfirmStopLearning = 10,
        ConfirmStartOverFromCoursePage = 11,
        GetNextLesson = 12
    }
}