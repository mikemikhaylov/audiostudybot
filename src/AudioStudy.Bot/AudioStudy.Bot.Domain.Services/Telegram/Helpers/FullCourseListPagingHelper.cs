using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Courses;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Model.Telegram.CallbackData;
using AudioStudy.Bot.Domain.Services.Courses;
using AudioStudy.Bot.SharedUtils.Helpers;
using AudioStudy.Bot.SharedUtils.Localization;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public class FullCourseListPagingHelper : CoursePagingHelperBase, IFullCourseListPagingHelper
    {
        private readonly IBotLocalization _botLocalization;
        private readonly ICourseProvider _courseProvider;
        private const int PageSize = 1;

        public FullCourseListPagingHelper(IBotLocalization botLocalization,
            ICourseProvider courseProvider) : base(botLocalization)
        {
            _botLocalization = botLocalization;
            _courseProvider = courseProvider;
        }

        public Task<TelegramResponseMessage> GetFirstPageAsync(User user) => GetPageAsync(user, new OpenPageCallbackData(0, PageSize));

        protected override IReadOnlyList<Course> GetCourses(User user, int skip, int take)
        {
            var userCourses = new HashSet<string>(user.Courses?.Select(x => x.Id) ?? new string[] { });
            return _courseProvider.GetCourses(user.LearningLanguage, user.KnowsLanguage)
                .Select((course, index) => new {course, isUserCourse = userCourses.Contains(course.Id), index})
                .OrderBy(x => x.isUserCourse)
                .ThenBy(x => x.index)
                .Select(x => x.course)
                .Skip(skip)
                .Take(take)
                .ToList();
        }

        protected override string GetCoursesMessage(User user) => _botLocalization.CoursesMessage(user.Language);

        protected override string GetNoCoursesMessage(User user) => _botLocalization.NoCoursesMessage(user.Language);

        protected override TelegramInlineBtn[][] GetAdditionalTopButtons(User user, int page, int pageSize)
        {
            return new[]
            {
                new []{new TelegramInlineBtn(_botLocalization.FilterCoursesBtn(user.Language), TelegramCallbackDataBase.OpenFilterToString(page, pageSize))}
            };
        }

        protected override TelegramInlineBtn[][] GetAdditionalBottomButtons(User user, int page, int pageSize)
        {
            return null;
        }

        protected override string GetOpenCourseData(User user, int page, int pageSize)
        {
            return TelegramCallbackDataBase.OpenCourseToString(page, pageSize);
        }

        protected override string GetCourseName(User user, Course course)
        {
            var name = _courseProvider.GetCourseName(user.Language, course);
            return user.Courses?.Any(x => x.Id == course.Id) == true
                ? FormatHelper.EmojiStar + name
                : name;
        }

        protected override string GetOpenPageData(User user, int page, int pageSize)
        {
            return TelegramCallbackDataBase.OpenCoursesPageToString(page, pageSize);
        }
    }
}