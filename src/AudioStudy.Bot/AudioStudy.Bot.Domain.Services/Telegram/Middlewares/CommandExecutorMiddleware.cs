using System;
using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services.Telegram.Helpers;
using AudioStudy.Bot.Domain.Services.Telegram.Middlewares.MenuSubMiddlewares;
using AudioStudy.Bot.SharedUtils.Localization;
using AudioStudy.Bot.SharedUtils.Metrics;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares
{
    public class CommandExecutorMiddleware : ITelegramPipelineMiddleware
    {
        private readonly IBotLocalization _botLocalization;
        private readonly IMenuSubMiddlewareFactory _menuSubMiddlewareFactory;
        private readonly IFullCourseListPagingHelper _fullCourseListPagingHelper;
        private readonly ILearnHelper _learnHelper;

        public CommandExecutorMiddleware(
            IBotLocalization botLocalization,
            IMenuSubMiddlewareFactory menuSubMiddlewareFactory,
            IFullCourseListPagingHelper fullCourseListPagingHelper,
            ILearnHelper learnHelper
            )
        {
            _botLocalization = botLocalization;
            _menuSubMiddlewareFactory = menuSubMiddlewareFactory;
            _fullCourseListPagingHelper = fullCourseListPagingHelper;
            _learnHelper = learnHelper;
        }

        public async Task HandleMessageAsync(TelegramPipelineContext pipelineContext)
        {
            string text = pipelineContext.RequestMessage.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            text = text.TrimStart();
            if (IsCommand(text, CommandConstants.StartCommand))
            {
                var sm = _menuSubMiddlewareFactory.Get(TelegramState.OnMainWindow);
                await sm.ChangeState(pipelineContext);
                pipelineContext.Intent = Intent.StartCommand;
                pipelineContext.Processed = true;
            }
            else if (IsCommand(text, CommandConstants.HelpCommand))
            {
                pipelineContext.ResponseMessage = new TelegramResponseMessage
                {
                    Text = _botLocalization.Help(pipelineContext.User.Language),
                    Html = true
                };
                pipelineContext.Processed = true;
                pipelineContext.Intent = Intent.HelpOpen;
            }
            else if (IsCommand(text, CommandConstants.SettingsCommand))
            {
                IMenuSubMiddleware sm = _menuSubMiddlewareFactory.Get(TelegramState.OnSettingsWindow);
                await sm.ChangeState(pipelineContext);
                pipelineContext.Intent = Intent.SettingsScreenOpen;
                pipelineContext.Processed = true;
            }
            else if (IsCommand(text, CommandConstants.CoursesCommand))
            {
                pipelineContext.ResponseMessage
                    = await _fullCourseListPagingHelper.GetFirstPageAsync(pipelineContext.User);
                pipelineContext.Intent = Intent.CoursesList;
                pipelineContext.Processed = true;
            }
            else if (IsCommand(text, CommandConstants.LearnCommand))
            {
                pipelineContext.ResponseMessage
                    = await _learnHelper.GetLearnPage(pipelineContext.User);
                pipelineContext.Intent = Intent.LearnPage;
                pipelineContext.Processed = true;
            }
        }

        private static bool IsCommand(string text, string command)
        {
            return text.StartsWith(command, StringComparison.OrdinalIgnoreCase);
        }
    }
}
