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

        public InlineKeyboardMiddleware(IFilterHelper filterHelper,
            IFullCourseListPagingHelper fullCourseListPagingHelper,
            ICourseHelper courseHelper)
        {
            _filterHelper = filterHelper;
            _fullCourseListPagingHelper = fullCourseListPagingHelper;
            _courseHelper = courseHelper;
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