using System.Collections.Generic;

namespace AudioStudy.Bot.Domain.Model.Telegram.CallbackData
{
    public class SetKnowsLanguageCallbackData : TelegramCallbackDataBase
    {
        public string LearningLanguage { get; }
        public int Page { get; }
        public int PageSize { get; }

        public SetKnowsLanguageCallbackData(IEnumerable<string> data)
        {
            using IEnumerator<string> enumerator = data.GetEnumerator();
            enumerator.MoveNext();
            LearningLanguage = enumerator.Current;
            enumerator.MoveNext();
            Page = int.Parse(enumerator.Current);
            enumerator.MoveNext();
            PageSize = int.Parse(enumerator.Current);
        }

        public override string ToString()
        {
            return ToString(TelegramInlineBtnType.OpenCourseFilter, LearningLanguage, Page.ToString(), PageSize.ToString());
        }
    }
}