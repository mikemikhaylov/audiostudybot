using System.Linq;
using AudioStudy.Bot.SharedUtils.Helpers;

namespace AudioStudy.Bot.Domain.Model.Telegram.CallbackData
{
    public abstract class TelegramCallbackDataBase
    {
        private static readonly string Delimiter = new string(FormatHelper.TelegramCallbackDataDelimiter, 1);

        public static string ToString(TelegramInlineBtnType telegramInlineBtnType, params string[] data)
        {
            if (data?.Any() == true)
            {
                return $"{(int)telegramInlineBtnType}{Delimiter}" + string.Join(Delimiter, data);
            }
            return ((int)telegramInlineBtnType).ToString();
        }
        
        public static string[] Parse(string data)
        {
            return data.Split(Delimiter);
        }

        public static string OpenCoursesPageToString(int page, int pageSize)
        {
            return ToString(TelegramInlineBtnType.OpenCourseListPage, page.ToString(), pageSize.ToString());
        }

        public static string OpenFilterToString(int page, int pageSize)
        {
            return ToString(TelegramInlineBtnType.OpenCourseFilter, page.ToString(), pageSize.ToString());
        }
        
        public static string OpenKnowsLanguageFilterToString(string learningLanguage, int page, int pageSize)
        {
            return ToString(TelegramInlineBtnType.OpenKnowsLanguageFilter, learningLanguage, page.ToString(), pageSize.ToString());
        }
        
        public static string SetCourseFilterToString(string learningLanguage, string knowsLanguage)
        {
            return ToString(TelegramInlineBtnType.SetCourseFilter, learningLanguage, knowsLanguage);
        }
         
        public static string CourseActionToString(TelegramInlineBtnType btnType, string courseId, int page, int pageSize)
        {
            return ToString(btnType, courseId, page.ToString(), pageSize.ToString());
        }
    }
}