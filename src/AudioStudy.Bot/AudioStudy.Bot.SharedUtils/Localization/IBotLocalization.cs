using AudioStudy.Bot.SharedUtils.Localization.Enums;

namespace AudioStudy.Bot.SharedUtils.Localization
{
    public interface IBotLocalization
    {
        string ChooseLanguage(Language language);
        string TelegramChatTypeIsNotSupported();
        string UnexpectedErrorHasOccured(Language language);
        string Help(Language language);
        string CancelBtnLabel(Language language);
        string BackBtnLabel(Language language);
        string HaveRatedBtnLabel(Language language);
        string WillRateLaterBtnLabel(Language language);
        string WillNotRateBtnLabel(Language language);
        string SettingsBtnLabel(Language language);
        string LearnBtnLabel(Language language);˜
        string CoursesBtnLabel(Language language);
        string MyCoursesBtnLabel(Language language);
        string HelpBtnLabel(Language language);
        string CourseLanguage(Language language, string courseLanguage);
    }
}