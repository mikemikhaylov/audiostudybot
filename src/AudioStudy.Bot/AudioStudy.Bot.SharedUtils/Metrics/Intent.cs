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
        HomeScreenOpen,
        LanguagesSettingsScreenOpen,
        SetLanguage,
        HelpOpen,
        StartCommand,
        RateAskScreenOpen,
        HaveRated,
        WillRateLater,
        WillNotRate,
    }
}