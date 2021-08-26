using System.Collections.Generic;

namespace AudioStudy.Bot.Domain.Model.Telegram.CallbackData
{
    public class OpenCourseCardsPageCallbackData : TelegramCallbackDataBase
    {
        public string CourseId { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int CourseListPage { get; }
        public int CourseListPageSize { get; }

        public OpenCourseCardsPageCallbackData(string courseId, int page, int pageSize, int courseListPage,
            int courseListPageSize)
        {
            CourseId = courseId;
            Page = page;
            PageSize = pageSize;
            CourseListPage = courseListPage;
            CourseListPageSize = courseListPageSize;
        }

        public OpenCourseCardsPageCallbackData(IEnumerable<string> data)
        {
            using IEnumerator<string> enumerator = data.GetEnumerator();
            enumerator.MoveNext();
            CourseId = enumerator.Current;
            enumerator.MoveNext();
            Page = int.Parse(enumerator.Current);
            enumerator.MoveNext();
            PageSize = int.Parse(enumerator.Current);
            enumerator.MoveNext();
            CourseListPage = int.Parse(enumerator.Current);
            enumerator.MoveNext();
            CourseListPageSize = int.Parse(enumerator.Current);
        }

        public override string ToString()
        {
            return ToString(TelegramInlineBtnType.OpenCourseCardsPage, CourseId, Page.ToString(), PageSize.ToString(),
                CourseListPage.ToString(), CourseListPageSize.ToString());
        }
    }
}