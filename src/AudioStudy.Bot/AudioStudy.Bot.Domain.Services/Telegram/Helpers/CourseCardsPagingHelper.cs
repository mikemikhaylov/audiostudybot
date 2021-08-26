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
    public class CourseCardsPagingHelper : CardsPagingHelperBase<OpenCourseCardsPageCallbackData>, ICourseCardsPagingHelper
    {
        private readonly IBotLocalization _botLocalization;
        private readonly ICourseProvider _courseProvider;

        public CourseCardsPagingHelper(IBotLocalization botLocalization,
            ICourseProvider courseProvider) : base(botLocalization)
        {
            _botLocalization = botLocalization;
            _courseProvider = courseProvider;
        }

        public Task<TelegramResponseMessage> GetPageAsync(User user, OpenCourseCardsPageCallbackData data)
        {
            return base.GetPageAsync(user, data.Page, data.PageSize, data);
        }

        protected override IReadOnlyList<Card> GetCards(User user, int skip, int take, OpenCourseCardsPageCallbackData data)
        {
            var course = _courseProvider.GetCourse(data.CourseId);
            return (course?.Cards ?? Array.Empty<Card>()).Skip(skip).Take(take).ToList();
        }

        protected override string GetFile(User user, OpenCourseCardsPageCallbackData data)
        {
            return null;
        }

        protected override string GetNoCardsMessage(User user)
        {
            return _botLocalization.NoCardsInCourse(user.Language);
        }

        protected override TelegramInlineBtn[][] GetAdditionalTopButtons(User user, int page, int pageSize, OpenCourseCardsPageCallbackData data)
        {
            return null;
        }

        protected override TelegramInlineBtn[][] GetAdditionalBottomButtons(User user, int page, int pageSize, OpenCourseCardsPageCallbackData data)
        {
            return new[]
            {
                new[]
                {
                    new TelegramInlineBtn(_botLocalization.BackToTheCourse(user.Language),
                        new OpenCourseCallbackData(data.CourseId, data.CourseListPage, data.CourseListPageSize).ToString())
                }
            };
        }

        protected override string GetOpenPageData(User user, int page, int pageSize, OpenCourseCardsPageCallbackData data)
        {
            return new OpenCourseCardsPageCallbackData(data.CourseId, page, pageSize, data.CourseListPage,
                data.CourseListPageSize).ToString();
        }
    }
}