using System.Collections.Generic;

namespace AudioStudy.Bot.Domain.Model.Telegram.CallbackData
{
    public class OpenLessonCardsPageCallbackData : TelegramCallbackDataBase
    {
        protected virtual TelegramInlineBtnType BtnType => TelegramInlineBtnType.OpenLessonCardsPage;
        public string CourseId { get; }
        public int Version { get; set; }
        public int Lesson { get; set; }
        public int Page { get; }
        public int PageSize { get; }

        public OpenLessonCardsPageCallbackData(string courseId, int version, int lesson, int page, int pageSize)
        {
            CourseId = courseId;
            Version = version;
            Lesson = lesson;
            Page = page;
            PageSize = pageSize;
        }

        public OpenLessonCardsPageCallbackData(IEnumerable<string> data)
        {
            using IEnumerator<string> enumerator = data.GetEnumerator();
            enumerator.MoveNext();
            CourseId = enumerator.Current;
            enumerator.MoveNext();
            Version = int.Parse(enumerator.Current);
            enumerator.MoveNext();
            Lesson = int.Parse(enumerator.Current);
            enumerator.MoveNext();
            Page = int.Parse(enumerator.Current);
            enumerator.MoveNext();
            PageSize = int.Parse(enumerator.Current);
        }

        public override string ToString()
        {
            return ToString(BtnType, CourseId, Version.ToString(), Lesson.ToString(), Page.ToString(), PageSize.ToString());
        }
    }
}