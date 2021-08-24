using System.Collections.Generic;

namespace AudioStudy.Bot.Domain.Model.Telegram.CallbackData
{
    public class SetFilterCallbackData : TelegramCallbackDataBase
    {
        public string LearningLanguage { get; }
        public string KnowsLanguage { get; }

        public SetFilterCallbackData(IEnumerable<string> data)
        {
            using IEnumerator<string> enumerator = data.GetEnumerator();
            enumerator.MoveNext();
            LearningLanguage = enumerator.Current;
            enumerator.MoveNext();
            KnowsLanguage = enumerator.Current;
        }

        public override string ToString()
        {
            return ToString(TelegramInlineBtnType.OpenCourseFilter, LearningLanguage, KnowsLanguage);
        }
    }
}