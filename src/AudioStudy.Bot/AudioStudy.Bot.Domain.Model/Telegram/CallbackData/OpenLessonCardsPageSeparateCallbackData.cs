using System.Collections.Generic;

namespace AudioStudy.Bot.Domain.Model.Telegram.CallbackData
{
    public class OpenLessonCardsPageSeparateCallbackData : OpenLessonCardsPageCallbackData
    {
        protected override TelegramInlineBtnType BtnType => TelegramInlineBtnType.OpenLessonCardsPageSeparateMessage;
        public OpenLessonCardsPageSeparateCallbackData(string courseId, int version, int lesson, int page, int pageSize)
            : base(courseId, version, lesson, page, pageSize)
        {
        }

        public OpenLessonCardsPageSeparateCallbackData(IEnumerable<string> data) : base(data)
        {
        }
    }
}