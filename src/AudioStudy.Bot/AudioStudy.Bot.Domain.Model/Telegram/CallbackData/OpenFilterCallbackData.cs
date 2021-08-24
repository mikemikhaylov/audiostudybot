using System.Collections.Generic;

namespace AudioStudy.Bot.Domain.Model.Telegram.CallbackData
{
    public class OpenFilterCallbackData : TelegramCallbackDataBase
    {
        public int Page { get; }
        public int PageSize { get; }

        public OpenFilterCallbackData(IEnumerable<string> data)
        {
            using IEnumerator<string> enumerator = data.GetEnumerator();
            enumerator.MoveNext();
            Page = int.Parse(enumerator.Current);
            enumerator.MoveNext();
            PageSize = int.Parse(enumerator.Current);
        }

        public override string ToString()
        {
            return ToString(TelegramInlineBtnType.OpenCourseFilter, Page.ToString(), PageSize.ToString());
        }
    }
}