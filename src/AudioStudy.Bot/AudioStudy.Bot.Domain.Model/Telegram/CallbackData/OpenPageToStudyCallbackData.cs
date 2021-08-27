using System.Collections.Generic;

namespace AudioStudy.Bot.Domain.Model.Telegram.CallbackData
{
    public class OpenPageToStudyCallbackData : TelegramCallbackDataBase
    {
        public int Page { get; }
        public int PageSize { get; }

        public OpenPageToStudyCallbackData(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }

        public OpenPageToStudyCallbackData(IEnumerable<string> data)
        {
            using IEnumerator<string> enumerator = data.GetEnumerator();
            enumerator.MoveNext();
            Page = int.Parse(enumerator.Current);
            enumerator.MoveNext();
            PageSize = int.Parse(enumerator.Current);
        }

        public override string ToString()
        {
            return ToString(TelegramInlineBtnType.OpenCourseListPageToStudy, Page.ToString(), PageSize.ToString());
        }
    }
}