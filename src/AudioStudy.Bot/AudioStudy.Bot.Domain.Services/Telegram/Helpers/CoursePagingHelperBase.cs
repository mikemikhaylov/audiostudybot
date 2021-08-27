using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Courses;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Model.Telegram.CallbackData;
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
            var courses = GetCourses(user, skip, pageSize + 1);
            string responseText;
            TelegramInlineBtn[][] noCoursesButtons = null;
            if (courses.Any())
            {
                responseText = GetCoursesMessage(user);
            }
            else
            {
                if (page == 0)
                {
                    responseText = GetNoCoursesMessage(user);
                    noCoursesButtons = GetNoCoursesButtons(user);
                }
                else
                {
                    responseText = _botLocalization.NoCoursesOnCurrentPage(user.Language);
                }
            }

            var result = new TelegramResponseMessage
            {
                Text = responseText
            };
            if (noCoursesButtons != null)
            {
                result.InlineButtons = noCoursesButtons;
                return Task.FromResult(result);
            }
            TelegramInlineBtn[][] coursesBtns = null;
            TelegramInlineBtn[] pagesBtns = null;
            if (courses.Any())
            {
                coursesBtns = courses
                    .Take(pageSize)
                    .Select((x, i) =>
                        new[]
                        {
                            new TelegramInlineBtn(GetCourseName(user, x),
                                GetOpenCourseData(user, x.Id, page, pageSize))
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

            var additionalTop = GetAdditionalTopButtons(user, page, pageSize);
            var additionalBottom = GetAdditionalBottomButtons(user, page, pageSize);
            
            var buttons = new[] {additionalTop, coursesBtns, pagesBtns == null ? null : new[] {pagesBtns}, additionalBottom}
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

        protected virtual TelegramInlineBtn[][] GetNoCoursesButtons(User user)
        {
            return null;
        }
        protected abstract TelegramInlineBtn[][] GetAdditionalTopButtons(User user, int page, int pageSize);
        protected abstract TelegramInlineBtn[][] GetAdditionalBottomButtons(User user, int page, int pageSize);
        protected abstract string GetOpenCourseData(User user, string courseId, int page, int pageSize);
        protected abstract string GetCourseName(User user, Course course);
        protected abstract string GetOpenPageData(User user, int page, int pageSize);
    }
}