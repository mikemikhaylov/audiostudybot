using System.Collections.Generic;

namespace AudioStudy.Bot.Domain.Model.Telegram.CallbackData
{
    public class StartOverCallbackData : TelegramCallbackDataBase
    {
        public string CourseId { get; }
        
        public StartOverCallbackData(string courseId)
        {
            CourseId = courseId;
        }

        public StartOverCallbackData(IEnumerable<string> data)
        {
            using IEnumerator<string> enumerator = data.GetEnumerator();
            enumerator.MoveNext();
            CourseId = enumerator.Current;
        }

        public override string ToString()
        {
            return ToString(TelegramInlineBtnType.StartOver, CourseId);
        }
    }
}