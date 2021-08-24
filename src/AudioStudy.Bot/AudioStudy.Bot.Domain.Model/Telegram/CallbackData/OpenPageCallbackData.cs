using System.Collections.Generic;

namespace AudioStudy.Bot.Domain.Model.Telegram.CallbackData
{
    public class OpenPageCallbackData: TelegramCallbackDataBase
    {
        public int Page { get; }
        public int PageSize { get; }

        public OpenPageCallbackData(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
        public OpenPageCallbackData(IEnumerable<string> data)
        {
            using IEnumerator<string> enumerator = data.GetEnumerator();
            enumerator.MoveNext();
            Page = int.Parse(enumerator.Current);
            enumerator.MoveNext();
            PageSize = int.Parse(enumerator.Current);
        }

        public override string ToString()
        {
            return ToString(TelegramInlineBtnType.OpenCourseListPage, Page.ToString(), PageSize.ToString());
        }
    }
}