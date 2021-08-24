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
        RateAskScreenOpen,
        HaveRated,
        WillRateLater,
        WillNotRate,
    }
}