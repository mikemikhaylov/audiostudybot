using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Courses;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.SharedUtils.Localization;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public abstract class CoursePagingHelperBase
    {
        private const string PrevPageFormat = "<< {0} {1}";
        private const string NextPageFormat = "{0} {1} >>";
        private readonly IBotLocalization _botLocalization;

        protected CoursePagingHelperBase(IBotLocalization botLocalization)
        {
            _botLocalization = botLocalization;
        }

        public Task<TelegramResponseMessage> GetPageAsync(User user, int page, int pageSize)
        {
            var skip = page * pageSize;
            var courses = GetCourses(user, skip, pageSize);
            string responseText;
            if (courses.Any())
            {
                responseText = GetCoursesMessage(user);
            }
            else
            {
                responseText = page == 0
                    ? GetNoCoursesMessage(user)
                    : _botLocalization.NoRemindersOnCurrentPage(user.Language);
            }

            var result = new TelegramResponseMessage
            {
                Text = responseText
            };
            TelegramInlineBtn[][] coursesBtns = null;
            TelegramInlineBtn[] pagesBtns = null;
            if (courses.Any())
            {
                var coursesOnPage = courses.Count > pageSize ? pageSize : courses.Count;
                coursesBtns = courses
                    .Select((x, i) =>
                        new[]
                        {
                            new TelegramInlineBtn(GetCourseName(user, x),
                                GetOpenCourseData(user, coursesOnPage))
                        }).ToArray();
            }

            if (page == 0)
            {
                if (courses.Count > pageSize)
                {
                    pagesBtns = new[]
                    {
                        new TelegramInlineBtn(
                            string.Format(NextPageFormat, _botLocalization.PageBtnLabel(user.Language), 2),
                            GetOpenPageData(user, 1, pageSize))
                    };
                }
            }
            else
            {
                if (courses.Count > pageSize)
                {
                    pagesBtns = new[]
                    {
                        new TelegramInlineBtn(
                            string.Format(PrevPageFormat, _botLocalization.PageBtnLabel(user.Language), page),
                            GetOpenPageData(user, page - 1, pageSize)),
                        new TelegramInlineBtn(
                            string.Format(NextPageFormat, _botLocalization.PageBtnLabel(user.Language), page + 2),
                            GetOpenPageData(user, page + 1, pageSize))
                    };
                }
                else
                {
                    pagesBtns = new[]
                    {
                        new TelegramInlineBtn(
                            string.Format(PrevPageFormat, _botLocalization.PageBtnLabel(user.Language), page),
                            GetOpenPageData(user, page - 1, pageSize))
                    };
                }
            }

            var additionalBottom = GetAdditionalBottomButtons(user);
            var buttons = new[] {coursesBtns, pagesBtns == null ? null : new[] {pagesBtns}, additionalBottom}
                .Where(x => x != null).SelectMany(x => x).ToArray();
            if (buttons.Any())
            {
                result.InlineButtons = buttons;
            }

            return Task.FromResult(result);
        }

        protected abstract IReadOnlyList<Course> GetCourses(User user, int skip, int take);
        protected abstract string GetCoursesMessage(User user);
        protected abstract string GetNoCoursesMessage(User user);
        protected abstract TelegramInlineBtn[][] GetAdditionalBottomButtons(User user);
        protected abstract string GetOpenCourseData(User user, int coursesOnPage);
        protected abstract string GetCourseName(User user, Course course);
        protected abstract string GetOpenPageData(User user, int page, int pageSize);
    }
}