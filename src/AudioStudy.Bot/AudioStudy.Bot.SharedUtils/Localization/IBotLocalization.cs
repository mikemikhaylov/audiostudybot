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
    }
}