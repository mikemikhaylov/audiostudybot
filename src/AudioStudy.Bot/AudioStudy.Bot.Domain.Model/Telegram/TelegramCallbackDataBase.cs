using System.Linq;
using AudioStudy.Bot.SharedUtils.Helpers;

namespace AudioStudy.Bot.Domain.Model.Telegram
{
    public abstract class TelegramCallbackDataBase
    {
        private static readonly string Delimiter = new string(FormatHelper.TelegramCallbackDataDelimiter, 1);

        private static string ToString(TelegramInlineBtnType telegramInlineBtnType, params string[] data)
        {
            if (data?.Any() == true)
            {
                return $"{(int)telegramInlineBtnType}{Delimiter}" + string.Join(Delimiter, data);
            }
            return ((int)telegramInlineBtnType).ToString();
        }

        public static string OpenCoursesPageToString(int page, int pageSize)
        {
            return ToString(TelegramInlineBtnType.OpenCoursesPage, page.ToString(), pageSize.ToString());
        }
         
        public static string CourseActionToString(TelegramInlineBtnType btnType, string courseId, int page, int pageSize, int coursesOnPage)
        {
            return ToString(btnType, courseId, page.ToString(), pageSize.ToString(), coursesOnPage.ToString());
        }
    }
}