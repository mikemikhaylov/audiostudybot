using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services.Telegram.Helpers;
using AudioStudy.Bot.SharedUtils.Localization;
using AudioStudy.Bot.SharedUtils.Metrics;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares.MenuSubMiddlewares
{
    [MenuSubMiddlewareMetadata(TelegramState = TelegramState.OnSettingsWindow)]
    public class SettingsSubMiddleware : IMenuSubMiddleware
    {
        private readonly ITelegramButtonsHelper _buttonsHelper;
        private readonly IBotLocalization _botLocalization;
        private readonly IMenuSubMiddlewareFactory _menuSubMiddlewareFactory;
        private readonly IUserService _userService;

        public SettingsSubMiddleware(ITelegramButtonsHelper buttonsHelper,
            IBotLocalization botLocalization,
            IMenuSubMiddlewareFactory menuSubMiddlewareFactory,
            IUserService userService
        )
        {
            _buttonsHelper = buttonsHelper;
            _botLocalization = botLocalization;
            _menuSubMiddlewareFactory = menuSubMiddlewareFactory;
            _userService = userService;
        }

        public async Task ChangeState(TelegramPipelineContext pipelineContext, UserUpdateCommand updateCommand = null)
        {
            pipelineContext.ResponseMessage = pipelineContext.ResponseMessage
                .AddText(_botLocalization.YourSettings(pipelineContext.User.Language))
                .SetReplyButtons(_buttonsHelper.GetStateButtons(TelegramState.OnSettingsWindow, pipelineContext.User));
            pipelineContext.ResponseMessage.Html = true;
            updateCommand = updateCommand.Combine((uc, fu) => uc.State = fu, TelegramState.OnSettingsWindow);
            await _userService.UpdateAsync(pipelineContext.User, updateCommand);
        }

        public async Task ProcessState(TelegramPipelineContext pipelineContext)
        {
            if (pipelineContext.RequestMessage.Text == _botLocalization.LanguageBtnLabel(pipelineContext.User.Language))
            {
                IMenuSubMiddleware sm = _menuSubMiddlewareFactory.Get(TelegramState.AwaitingForLanguage);
                await sm.ChangeState(pipelineContext);
                pipelineContext.Intent = Intent.LanguagesSettingsScreenOpen;
            }
            else if (pipelineContext.RequestMessage.Text == _botLocalization.HelpBtnLabel(pipelineContext.User.Language))
            {
                pipelineContext.ResponseMessage = pipelineContext.ResponseMessage.AddText(_botLocalization.Help(pipelineContext.User.Language));
                pipelineContext.Intent = Intent.HelpOpen;
            }
            else if (pipelineContext.RequestMessage.Text == _botLocalization.BackBtnLabel(pipelineContext.User.Language))
            {
                IMenuSubMiddleware sm = _menuSubMiddlewareFactory.Get(TelegramState.OnMainWindow);
                await sm.ChangeState(pipelineContext);
                pipelineContext.Intent = Intent.MainScreenOpen;
            }
            else
            {
                pipelineContext.ResponseMessage = pipelineContext.ResponseMessage.SetReplyButtons(
                        _buttonsHelper.GetStateButtons(TelegramState.OnSettingsWindow, pipelineContext.User))
                    .AddText(_botLocalization.UnknownCommand(pipelineContext.User.Language));
            }
            pipelineContext.Processed = true;
        }
    }
}