using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Courses;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Model.Telegram.CallbackData;
using AudioStudy.Bot.Domain.Services.Courses;
using AudioStudy.Bot.SharedUtils.Localization;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public class LearnHelper : CoursePagingHelperBase, ILearnHelper
    {
        private readonly IBotLocalization _botLocalization;
        private readonly ICourseProvider _courseProvider;
        private readonly IUserService _userService;

        public LearnHelper(IBotLocalization botLocalization,
            ICourseProvider courseProvider,
            IUserService userService) : base(botLocalization)
        {
            _botLocalization = botLocalization;
            _courseProvider = courseProvider;
            _userService = userService;
        }

        private Task<TelegramResponseMessage> GetFirstPageAsync(User user)
        {
            return GetPageAsync(user, new OpenPageToStudyCallbackData(0, Consts.CoursePerPage));
        }

        public async Task<TelegramResponseMessage> GetPageAsync(User user, OpenPageToStudyCallbackData data)
        {
            return await GetPageAsync(user, data.Page, data.PageSize);
        }

        public async Task<TelegramResponseMessage> GetLearnPage(User user)
        {
            if (string.IsNullOrWhiteSpace(user.LearningCourseId))
            {
                return await GetFirstPageAsync(user);
            }

            var course = _courseProvider.GetCourse(user.LearningCourseId);
            if (course == null)
            {
                await _userService.UpdateAsync(user, UserUpdateCommand.Factory.UpdateLearningCourse(null));
                return await GetFirstPageAsync(user);
            }

            return new TelegramResponseMessage
            {
                Text = _botLocalization.CurrentlyLearningCourse(user.Language,
                    (_courseProvider.GetCourseName(user.Language, course), 0)),
                InlineButtons = new[]
                {
                    new[]
                    {
                        new TelegramInlineBtn(_botLocalization.GetNextLesson(user.Language),
                            new GetNextLessonCallbackData(course.Id).ToString())
                    },
                    new[]
                    {
                        new TelegramInlineBtn(_botLocalization.ChooseAnotherCourse(user.Language),
                            new OpenPageToStudyCallbackData(0, Consts.CoursePerPage).ToString())
                    }
                }
            };
        }

        public async Task<TelegramResponseMessage> SetCourseToLearn(User user, SetCourseToLearnCallbackData data)
        {
            await _userService.UpdateAsync(user, UserUpdateCommand.Factory.UpdateLearningCourse(data.CourseId));
            return await GetLearnPage(user);
        }

        protected override IReadOnlyList<Course> GetCourses(User user, int skip, int take)
        {
            return (user.Courses ?? Array.Empty<UserCourse>()).Select(x => _courseProvider.GetCourse(x.Id))
                .Where(x => x != null).Skip(skip).Take(take).ToList();
        }

        protected override string GetCoursesMessage(User user)
        {
            return _botLocalization.CoursesToLearnMessage(user.Language);
        }

        protected override string GetNoCoursesMessage(User user)
        {
            return _botLocalization.NoCoursesToLearnMessage(user.Language);
        }

        protected override TelegramInlineBtn[][] GetNoCoursesButtons(User user)
        {
            return null;
        }
        
        protected override TelegramInlineBtn[][] GetAdditionalTopButtons(User user, int page, int pageSize)
        {
            return null;
        }

        protected override TelegramInlineBtn[][] GetAdditionalBottomButtons(User user, int page, int pageSize)
        {
            var buttons = new[]
            {
                new[]
                {
                    new TelegramInlineBtn(_botLocalization.InlineCoursesBtnLabel(user.Language),
                        new OpenPageCallbackData(0, Consts.CoursePerPage).ToString())
                }
            };
            if (string.IsNullOrWhiteSpace(user.LearningCourseId))
            {
                return buttons;
            }

            return new[]
            {
                new[]
                {
                    new TelegramInlineBtn(_botLocalization.InlineBackBtn(user.Language),
                        new OpenLearnPageCallbackData().ToString())
                }
            }.Concat(buttons).ToArray();
        }

        protected override string GetOpenCourseData(User user, string courseId, int page, int pageSize)
        {
            return new SetCourseToLearnCallbackData(courseId).ToString();
        }

        protected override string GetCourseName(User user, Course course)
        {
            return _courseProvider.GetCourseName(user.Language, course);
        }

        protected override string GetOpenPageData(User user, int page, int pageSize)
        {
            return new OpenPageToStudyCallbackData(page, pageSize).ToString();
        }
    }
}