using System.Collections.Generic;

namespace AudioStudy.Bot.Domain.Model.Telegram.CallbackData
{
    public class SetCourseToLearnCallbackData : TelegramCallbackDataBase
    {
        public string CourseId { get; }

        public SetCourseToLearnCallbackData(string courseId)
        {
            CourseId = courseId;
        }

        public SetCourseToLearnCallbackData(IEnumerable<string> data)
        {
            using IEnumerator<string> enumerator = data.GetEnumerator();
            enumerator.MoveNext();
            CourseId = enumerator.Current;
        }

        public override string ToString()
        {
            return ToString(TelegramInlineBtnType.SetCourseToLearn, CourseId);
        }
    }
}