using System.Collections.Generic;

namespace AudioStudy.Bot.Domain.Model.Telegram.CallbackData
{
    public class StartOverFromCoursePageCallbackData : TelegramCallbackDataBase
    {
        public string CourseId { get; }
        public int Page { get; }
        public int PageSize { get; }

        public StartOverFromCoursePageCallbackData(string courseId, int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
            CourseId = courseId;
        }

        public StartOverFromCoursePageCallbackData(IEnumerable<string> data)
        {
            using IEnumerator<string> enumerator = data.GetEnumerator();
            enumerator.MoveNext();
            CourseId = enumerator.Current;
            enumerator.MoveNext();
            Page = int.Parse(enumerator.Current);
            enumerator.MoveNext();
            PageSize = int.Parse(enumerator.Current);
        }

        public override string ToString()
        {
            return ToString(TelegramInlineBtnType.StartOverFromCoursePage, CourseId, Page.ToString(), PageSize.ToString());
        }
    }
}