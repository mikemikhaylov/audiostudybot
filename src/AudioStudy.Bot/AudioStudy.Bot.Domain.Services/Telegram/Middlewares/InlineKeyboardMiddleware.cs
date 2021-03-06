using System.Linq;
using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Model.Telegram.CallbackData;
using AudioStudy.Bot.Domain.Services.Telegram.Helpers;
using AudioStudy.Bot.SharedUtils.Metrics;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares
{
    public class InlineKeyboardMiddleware: ITelegramPipelineMiddleware
    {
        private readonly IFilterHelper _filterHelper;
        private readonly IFullCourseListPagingHelper _fullCourseListPagingHelper;
        private readonly ICourseHelper _courseHelper;
        private readonly ICourseCardsPagingHelper _courseCardsPagingHelper;
        private readonly ILessonCardsPagingHelper _lessonCardsPagingHelper;
        private readonly ILearnHelper _learnHelper;

        public InlineKeyboardMiddleware(IFilterHelper filterHelper,
            IFullCourseListPagingHelper fullCourseListPagingHelper,
            ICourseHelper courseHelper,
            ICourseCardsPagingHelper courseCardsPagingHelper,
            ILessonCardsPagingHelper lessonCardsPagingHelper,
            ILearnHelper learnHelper)
        {
            _filterHelper = filterHelper;
            _fullCourseListPagingHelper = fullCourseListPagingHelper;
            _courseHelper = courseHelper;
            _courseCardsPagingHelper = courseCardsPagingHelper;
            _lessonCardsPagingHelper = lessonCardsPagingHelper;
            _learnHelper = learnHelper;
        }
        public async Task HandleMessageAsync(TelegramPipelineContext pipelineContext)
        {
            if (pipelineContext.RequestMessage.CallbackMessageId == null)
            {
                return;
            }

            string[] data =
                TelegramCallbackDataBase.Parse(pipelineContext.RequestMessage.CallbackData);
            var btnType = (TelegramInlineBtnType)int.Parse(data[0]);
            TelegramResponseMessage responseMessage = null;
            var sendAsCallback = true;
            switch (btnType)
            {
                case TelegramInlineBtnType.OpenCourseListPage:
                    responseMessage = await _fullCourseListPagingHelper.GetPageAsync(pipelineContext.User,
                        new OpenPageCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.CoursesList;
                    break;
                case TelegramInlineBtnType.OpenCourseFilter:
                    responseMessage = _filterHelper.GetLearningLanguageFilter(pipelineContext.User,
                        new OpenFilterCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.OpenLearningLanguageFilter;
                    break;
                case TelegramInlineBtnType.OpenKnowsLanguageFilter:
                    responseMessage = _filterHelper.GetKnowsLanguageFilter(pipelineContext.User,
                        new SetKnowsLanguageCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.OpenKnowsLanguageFilter;
                    break;
                case TelegramInlineBtnType.SetCourseFilter:
                    responseMessage = await _filterHelper.SetFilter(pipelineContext.User,
                        new SetFilterCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.SetCourseFilter;
                    break;
                case TelegramInlineBtnType.OpenCourse:
                    responseMessage = _courseHelper.GetCoursePage(pipelineContext.User,
                        new OpenCourseCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.OpenCourse;
                    break;
                case TelegramInlineBtnType.StartLearning:
                    responseMessage = await _courseHelper.StartCourse(pipelineContext.User,
                        new StartLearningCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.StartLearning;
                    break;
                case TelegramInlineBtnType.StopLearning:
                    responseMessage = await _courseHelper.StopCourse(pipelineContext.User,
                        new StopLearningCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.StopLearning;
                    break;
                case TelegramInlineBtnType.StartOverFromCoursePage:
                    responseMessage = await _courseHelper.StartOverFromPageCourse(pipelineContext.User,
                        new StartOverFromCoursePageCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.StartCourseOver;
                    break;
                case TelegramInlineBtnType.ConfirmStopLearning:
                    responseMessage = await _courseHelper.ConfirmStopCourse(pipelineContext.User,
                        new ConfirmStopLearningCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.StopLearning;
                    break;
                case TelegramInlineBtnType.ConfirmStartOverFromCoursePage:
                    responseMessage = await _courseHelper.ConfirmStartOverCourse(pipelineContext.User,
                        new ConfirmStartOverFromCoursePageCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.StartCourseOver;
                    break;
                case TelegramInlineBtnType.StartOver:
                    responseMessage = await _courseHelper.StartOver(pipelineContext.User,
                        new StartOverCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.StartCourseOver;
                    break;
                case TelegramInlineBtnType.GetNextLesson:
                    responseMessage = await _courseHelper.GetNextLessonAsync(pipelineContext.User,
                        new GetNextLessonCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.GetNextLesson;
                    sendAsCallback = false;
                    break;
                case TelegramInlineBtnType.OpenCourseCardsPage:
                    responseMessage = await _courseCardsPagingHelper.GetPageAsync(pipelineContext.User,
                        new OpenCourseCardsPageCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.ShowCourseCards;
                    break;
                case TelegramInlineBtnType.OpenLessonCardsPage:
                    responseMessage = await _lessonCardsPagingHelper.GetPageAsync(pipelineContext.User,
                        new OpenLessonCardsPageCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.ShowLessonCards;
                    break;
                case TelegramInlineBtnType.OpenCourseListPageToStudy:
                    responseMessage = await _learnHelper.GetPageAsync(pipelineContext.User, new OpenPageToStudyCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.OpenCourseListToStudyPage;
                    break;
                case TelegramInlineBtnType.OpenLearnPage:
                    responseMessage = await _learnHelper.GetLearnPage(pipelineContext.User);
                    pipelineContext.Intent = Intent.LearnPage;
                    break;
                case TelegramInlineBtnType.SetCourseToLearn:
                    responseMessage = await _learnHelper.SetCourseToLearn(pipelineContext.User, new SetCourseToLearnCallbackData(data.Skip(1)));
                    pipelineContext.Intent = Intent.SetCourseToLearn;
                    break;
            }

            if (responseMessage != null)
            {
                responseMessage.CallbackQueryId = pipelineContext.RequestMessage.CallbackQueryId;
                pipelineContext.ResponseMessage = responseMessage;
                if (sendAsCallback)
                {
                    pipelineContext.ResponseMessage.CallbackMessageId = pipelineContext.RequestMessage.CallbackMessageId;
                }
            }
            pipelineContext.Processed = true;
        }
    }
}