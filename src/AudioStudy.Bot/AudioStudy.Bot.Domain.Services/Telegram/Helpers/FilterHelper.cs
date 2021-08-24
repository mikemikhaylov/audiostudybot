using System.Linq;
using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Model.Telegram.CallbackData;
using AudioStudy.Bot.Domain.Services.Courses;
using AudioStudy.Bot.SharedUtils.Localization;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public class FilterHelper : IFilterHelper
    {
        private readonly IBotLocalization _botLocalization;
        private readonly ICourseProvider _courseProvider;
        private readonly IFullCourseListPagingHelper _fullCourseListPagingHelper;
        private readonly IUserService _userService;

        public FilterHelper(
            IBotLocalization botLocalization, 
            ICourseProvider courseProvider, 
            IFullCourseListPagingHelper fullCourseListPagingHelper,
            IUserService userService)
        {
            _botLocalization = botLocalization;
            _courseProvider = courseProvider;
            _fullCourseListPagingHelper = fullCourseListPagingHelper;
            _userService = userService;
        }

        public TelegramResponseMessage GetLearningLanguageFilter(User user, OpenFilterCallbackData data)
        {
            return new TelegramResponseMessage
            {
                Text = _botLocalization.ChooseLearningLanguage(user.Language),
                InlineButtons = _courseProvider.GetCoursesLanguages()
                    .Select(x => new {language = x, button = _botLocalization.CourseLanguage(user.Language, x)})
                    .Select(x => new[]
                    {
                        new TelegramInlineBtn(x.button,
                            TelegramCallbackDataBase.OpenKnowsLanguageFilterToString(x.language, data.Page,
                                data.PageSize))
                    })
                    .Concat(new[]
                    {
                        new[]
                        {
                            new TelegramInlineBtn(_botLocalization.InlineBackBtn(user.Language),
                                TelegramCallbackDataBase.OpenCoursesPageToString(data.Page, data.PageSize))
                        }
                    })
                    .ToArray()
            };
        }

        public TelegramResponseMessage GetKnowsLanguageFilter(User user, SetKnowsLanguageCallbackData data)
        {
            return new TelegramResponseMessage
            {
                Text = _botLocalization.ChooseKnowsLanguage(user.Language),
                InlineButtons = _courseProvider.GetTranslationLanguages(data.LearningLanguage)
                    .Select(x => new {knowsLanguage = x, button = _botLocalization.CourseLanguage(user.Language, x)})
                    .Select(x => new[]
                    {
                        new TelegramInlineBtn(x.button,
                            TelegramCallbackDataBase.SetCourseFilterToString(data.LearningLanguage, x.knowsLanguage))
                    })
                    .Concat(new[]
                    {
                        new[]
                        {
                            new TelegramInlineBtn(_botLocalization.InlineBackBtn(user.Language),
                                TelegramCallbackDataBase.OpenFilterToString(data.Page, data.PageSize))
                        }
                    })
                    .ToArray()
            };
        }

        public async Task<TelegramResponseMessage> SetFilter(User user, SetFilterCallbackData data)
        {
            await _userService.UpdateAsync(user, new UserUpdateCommand
            {
                LearningLanguage = new FieldUpdateCommand<string>(data.LearningLanguage),
                KnowsLanguage = new FieldUpdateCommand<string>(data.KnowsLanguage)
            });
            return await _fullCourseListPagingHelper.GetFirstPageAsync(user);
        }
    }
}