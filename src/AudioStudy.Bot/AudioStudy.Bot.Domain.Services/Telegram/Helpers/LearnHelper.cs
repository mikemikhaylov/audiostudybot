using System.Collections.Generic;
using System.Threading.Tasks;
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
        public LearnHelper(IBotLocalization botLocalization,
            ICourseProvider courseProvider) : base(botLocalization)
        {
            _botLocalization = botLocalization;
            _courseProvider = courseProvider;
        }
        
        public Task<TelegramResponseMessage> GetFirstPageAsync(User user)
        {
            throw new System.NotImplementedException();
        }

        public Task<TelegramResponseMessage> GetPageAsync(User user, OpenPageToStudyCallbackData data)
        {
            throw new System.NotImplementedException();
        }

        public Task<TelegramResponseMessage> GetLearnPage(User user)
        {
            throw new System.NotImplementedException();
        }

        public Task<TelegramResponseMessage> SetCourseToLearn(User user, SetCourseToLearnCallbackData data)
        {
            throw new System.NotImplementedException();
        }

        public LearnHelper(IBotLocalization botLocalization) : base(botLocalization)
        {
        }

        protected override IReadOnlyList<Course> GetCourses(User user, int skip, int take)
        {
            throw new System.NotImplementedException();
        }

        protected override string GetCoursesMessage(User user)
        {
            throw new System.NotImplementedException();
        }

        protected override string GetNoCoursesMessage(User user)
        {
            throw new System.NotImplementedException();
        }

        protected override TelegramInlineBtn[][] GetAdditionalTopButtons(User user, int page, int pageSize)
        {
            return null;
        }

        protected override TelegramInlineBtn[][] GetAdditionalBottomButtons(User user, int page, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(user.LearningCourseId))
            {
                return null;
            }
            return new[]
            {
                new[]
                {
                    new TelegramInlineBtn(_botLocalization.InlineBackBtn(user.Language),
                        TelegramCallbackDataBase.OpenFilterToString(page, pageSize))
                }
            };
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