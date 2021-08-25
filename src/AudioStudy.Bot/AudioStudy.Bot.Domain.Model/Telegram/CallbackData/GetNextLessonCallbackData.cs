using System.Collections.Generic;

namespace AudioStudy.Bot.Domain.Model.Telegram.CallbackData
{
    public class GetNextLessonCallbackData : TelegramCallbackDataBase
    {
        public string CourseId { get; }

        public GetNextLessonCallbackData(string courseId)
        {
            CourseId = courseId;
        }

        public GetNextLessonCallbackData(IEnumerable<string> data)
        {
            using IEnumerator<string> enumerator = data.GetEnumerator();
            enumerator.MoveNext();
            CourseId = enumerator.Current;
        }

        public override string ToString()
        {
            return ToString(TelegramInlineBtnType.GetNextLesson, CourseId);
        }
    }
}