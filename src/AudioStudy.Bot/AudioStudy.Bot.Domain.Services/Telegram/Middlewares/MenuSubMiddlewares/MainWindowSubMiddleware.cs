using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services.Telegram.Helpers;
using AudioStudy.Bot.SharedUtils.Localization;
using AudioStudy.Bot.SharedUtils.Metrics;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares.MenuSubMiddlewares
{
    [MenuSubMiddlewareMetadata(TelegramState = TelegramState.OnMainWindow)]
    public class MainWindowSubMiddleware : IMenuSubMiddleware
    {
        private readonly ITelegramButtonsHelper _buttonsHelper;
        private readonly IUserService _userService;
        private readonly IMenuSubMiddlewareFactory _menuSubMiddlewareFactory;
        private readonly IBotLocalization _botLocalization;
        private readonly IFullCourseListPagingHelper _fullCourseListPagingHelper;

        public MainWindowSubMiddleware(ITelegramButtonsHelper buttonsHelper,
                                        IUserService userService,
                                        IMenuSubMiddlewareFactory menuSubMiddlewareFactory,
                                        IBotLocalization botLocalization,
                                        IFullCourseListPagingHelper fullCourseListPagingHelper)
        {
            _buttonsHelper = buttonsHelper;
            _userService = userService;
            _menuSubMiddlewareFactory = menuSubMiddlewareFactory;
            _botLocalization = botLocalization;
            _fullCourseListPagingHelper = fullCourseListPagingHelper;
        }

        public async Task ChangeState(TelegramPipelineContext pipelineContext, UserUpdateCommand updateCommand = null)
        {
            pipelineContext.ResponseMessage = pipelineContext.ResponseMessage.SetReplyButtons(
                _buttonsHelper.GetStateButtons(TelegramState.OnMainWindow, pipelineContext.User))
                .AddText("мы на главном меню");
            updateCommand = updateCommand.Combine((uc, fu) => uc.State = fu, TelegramState.OnMainWindow);
            await _userService.UpdateAsync(pipelineContext.User, updateCommand);
        }

        public async Task ProcessState(TelegramPipelineContext pipelineContext)
        {
            if (pipelineContext.RequestMessage.Text == _botLocalization.SettingsBtnLabel(pipelineContext.User.Language))
            {
                var sm = _menuSubMiddlewareFactory.Get(TelegramState.OnSettingsWindow);
                await sm.ChangeState(pipelineContext);
                pipelineContext.Intent = Intent.SettingsScreenOpen;
            }
            else if (pipelineContext.RequestMessage.Text == _botLocalization.CoursesBtnLabel(pipelineContext.User.Language))
            {
                pipelineContext.ResponseMessage
                    = await _fullCourseListPagingHelper.GetFirstPageAsync(pipelineContext.User);
                pipelineContext.Intent = Intent.CoursesList;
            }
            else
            {
                pipelineContext.ResponseMessage = pipelineContext.ResponseMessage.SetReplyButtons(
                    _buttonsHelper.GetStateButtons(TelegramState.OnMainWindow, pipelineContext.User)).AddText("непонятно");
            }
            pipelineContext.Processed = true;
        }
    }
}