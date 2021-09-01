using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services.Telegram.Helpers;
using AudioStudy.Bot.SharedUtils.Localization;
using AudioStudy.Bot.SharedUtils.Metrics;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares.MenuSubMiddlewares
{
    [MenuSubMiddlewareMetadata(TelegramState = TelegramState.AwaitingForLanguage)]
    public class LanguageSettingSubMiddleware: IMenuSubMiddleware
    {
        private readonly ITelegramButtonsHelper _buttonsHelper;
        private readonly ITelegramButtonsHelper _telegramButtonsHelper;
        private readonly IUserService _userService;
        private readonly IBotLocalization _botLocalization;
        private readonly IMenuSubMiddlewareFactory _menuSubMiddlewareFactory;

        public LanguageSettingSubMiddleware(ITelegramButtonsHelper buttonsHelper,
            ITelegramButtonsHelper telegramButtonsHelper,
            IUserService userService,
            IBotLocalization botLocalization,
            IMenuSubMiddlewareFactory menuSubMiddlewareFactory)
        {
            _buttonsHelper = buttonsHelper;
            _telegramButtonsHelper = telegramButtonsHelper;
            _userService = userService;
            _botLocalization = botLocalization;
            _menuSubMiddlewareFactory = menuSubMiddlewareFactory;
        }

        public async Task ChangeState(TelegramPipelineContext pipelineContext, UserUpdateCommand updateCommand = null)
        {
            pipelineContext.ResponseMessage = pipelineContext.ResponseMessage
                .AddText(_botLocalization.ChooseLanguage(pipelineContext.User.Language))
                .SetReplyButtons(_buttonsHelper.GetStateButtons(TelegramState.AwaitingForLanguage, pipelineContext.User));
            updateCommand = updateCommand.Combine((uc, fu) => uc.State = fu, TelegramState.AwaitingForLanguage);
            await _userService.UpdateAsync(pipelineContext.User, updateCommand);
        }

        public async Task ProcessState(TelegramPipelineContext pipelineContext)
        {
            if (_telegramButtonsHelper.TryGetLang(pipelineContext.RequestMessage.Text, out var language))
            {
                await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateLanguage(language));
                var menuSubMiddleware = _menuSubMiddlewareFactory.Get(TelegramState.OnSettingsWindow);
                await menuSubMiddleware.ChangeState(pipelineContext);
                pipelineContext.Intent = Intent.SetLanguage;
            }
            else if (pipelineContext.RequestMessage.Text == _botLocalization.CancelBtnLabel(pipelineContext.User.Language))
            {
                var sm = _menuSubMiddlewareFactory.Get(TelegramState.OnSettingsWindow);
                await sm.ChangeState(pipelineContext);
                pipelineContext.Intent = Intent.SettingsScreenOpen;
            }
            else
            {
                pipelineContext.ResponseMessage = pipelineContext.ResponseMessage.AddText(_botLocalization.UnknownCommand(pipelineContext.User.Language)).SetReplyButtons(
                    _buttonsHelper.GetStateButtons(TelegramState.AwaitingForLanguage, pipelineContext.User));
            }
            pipelineContext.Processed = true;
        }
    }
}