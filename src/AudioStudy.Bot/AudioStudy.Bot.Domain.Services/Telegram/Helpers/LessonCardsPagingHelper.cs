using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Courses;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Model.Telegram.CallbackData;
using AudioStudy.Bot.Domain.Services.Courses;
using AudioStudy.Bot.SharedUtils.Localization;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public class LessonCardsPagingHelper : CardsPagingHelperBase<OpenLessonCardsPageCallbackData>,
        ILessonCardsPagingHelper
    {
        private readonly IBotLocalization _botLocalization;
        private readonly ILessonProvider _lessonProvider;

        public LessonCardsPagingHelper(IBotLocalization botLocalization,
            ILessonProvider lessonProvider) : base(botLocalization)
        {
            _botLocalization = botLocalization;
            _lessonProvider = lessonProvider;
        }

        public Task<TelegramResponseMessage> GetPageAsync(User user, OpenLessonCardsPageCallbackData data)
        {
            return base.GetPageAsync(user, data.Page, data.PageSize, data);
        }

        protected override string GetFirstPageHeader(User user, OpenLessonCardsPageCallbackData data)
        {
            var numberOfLessons = _lessonProvider.GetCourseLessons(data.CourseId, data.Version).Length;
            return _botLocalization.LessonNOfN(user.Language, numberOfLessons, data.Lesson + 1);
        }

        protected override IReadOnlyList<Card> GetCards(User user, int skip, int take,
            OpenLessonCardsPageCallbackData data)
        {
            if (data.Lesson < 0)
            {
                return Array.Empty<Card>();
            }

            var lessons = _lessonProvider.GetCourseLessons(data.CourseId, data.Version);
            if (lessons.Length <= data.Lesson)
            {
                return Array.Empty<Card>();
            }

            return (lessons[data.Lesson].Cards ?? Array.Empty<Card>()).Skip(skip).Take(take).ToList();
        }

        protected override string GetFile(User user, OpenLessonCardsPageCallbackData data)
        {
            if (data.Lesson < 0)
            {
                return null;
            }

            var lessons = _lessonProvider.GetCourseLessons(data.CourseId, data.Version);
            if (lessons.Length <= data.Lesson)
            {
                return null;
            }

            return lessons[data.Lesson].FileId;
        }

        protected override string GetNoCardsMessage(User user)
        {
            return _botLocalization.NoCardsInCourse(user.Language);
        }

        protected override TelegramInlineBtn[][] GetAdditionalTopButtons(User user, int page, int pageSize,
            OpenLessonCardsPageCallbackData data)
        {
            return null;
        }

        protected override TelegramInlineBtn[][] GetAdditionalBottomButtons(User user, int page, int pageSize,
            OpenLessonCardsPageCallbackData data)
        {
            return new[]
            {
                new[]
                {
                    new TelegramInlineBtn(_botLocalization.GetNextLesson(user.Language),
                        new GetNextLessonCallbackData(data.CourseId).ToString())
                }
            };
        }

        protected override string GetOpenPageData(User user, int page, int pageSize,
            OpenLessonCardsPageCallbackData data)
        {
            return new OpenLessonCardsPageCallbackData(data.CourseId, data.Version, data.Lesson, page, pageSize)
                .ToString();
        }
    }
}