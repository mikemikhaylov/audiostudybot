namespace AudioStudy.Bot.SharedUtils.Metrics
{
    public enum Intent
    {
        Unknown = 0,
        Initial,
        FirstLanguageSet,
        FirstLearningLanguageSet,
        FirstKnowsLanguageSet,
        SettingsScreenOpen,
        MainScreenOpen,
        LanguagesSettingsScreenOpen,
        SetLanguage,
        HelpOpen,
        StartCommand,
        CoursesList,
        OpenLearningLanguageFilter,
        OpenKnowsLanguageFilter,
        SetCourseFilter,
        OpenCourse,
        RateAskScreenOpen,
        HaveRated,
        WillRateLater,
        WillNotRate,
    }
}