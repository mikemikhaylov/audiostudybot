using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services.Telegram.Helpers;
using AudioStudy.Bot.SharedUtils.Localization;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares.MenuSubMiddlewares
{
    [MenuSubMiddlewareMetadata(TelegramState = TelegramState.OnMainWindow)]
    public class MainWindowSubMiddleware : IMenuSubMiddleware
    {
        private readonly ITelegramButtonsHelper _buttonsHelper;
        private readonly IUserService _userService;

        public MainWindowSubMiddleware(ITelegramButtonsHelper buttonsHelper,
                                        IUserService userService)
        {
            _buttonsHelper = buttonsHelper;
            _userService = userService;
        }

        public async Task ChangeState(TelegramPipelineContext pipelineContext, UserUpdateCommand updateCommand = null)
        {
            pipelineContext.ResponseMessage.SetReplyButtons(
                _buttonsHelper.GetStateButtons(TelegramState.OnMainWindow, pipelineContext.User));
            updateCommand = updateCommand.Combine((uc, fu) => uc.State = fu, TelegramState.OnMainWindow);
            await _userService.UpdateAsync(pipelineContext.User, updateCommand);
        }

        public async Task ProcessState(TelegramPipelineContext pipelineContext)
        {
            pipelineContext.Processed = true;
        }
    }
}