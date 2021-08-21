using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services.Telegram.Helpers;
using AudioStudy.Bot.Domain.Services.Telegram.Middlewares.MenuSubMiddlewares;
using AudioStudy.Bot.SharedUtils.Localization;
using AudioStudy.Bot.SharedUtils.Localization.Enums;
using AudioStudy.Bot.SharedUtils.Metrics;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares
{
    public class SettingsCheckerMiddleware : ITelegramPipelineMiddleware
    {
        private readonly ITelegramButtonsHelper _telegramButtonsHelper;
        private readonly IUserService _userService;
        private readonly IBotLocalization _botLocalization;
        private readonly IMenuSubMiddlewareFactory _menuSubMiddlewareFactory;

        public SettingsCheckerMiddleware(
                                            ITelegramButtonsHelper telegramButtonsHelper,
                                            IUserService userService,
                                            IBotLocalization botLocalization,
                                            IMenuSubMiddlewareFactory menuSubMiddlewareFactory)
        {
            _telegramButtonsHelper = telegramButtonsHelper;
            _userService = userService;
            _botLocalization = botLocalization;
            _menuSubMiddlewareFactory = menuSubMiddlewareFactory;
        }

        public async Task HandleMessageAsync(TelegramPipelineContext pipelineContext)
        {
            var user = pipelineContext.User;
            var settingsSet = user.Language == Language.Unknown 
                               || string.IsNullOrWhiteSpace(user.LearningLanguage) 
                               || string.IsNullOrWhiteSpace(user.KnowsLanguage);
            pipelineContext.Processed = settingsSet;
            settingsSet = await HandleUnknownLanguageAsync(pipelineContext);
            settingsSet = settingsSet && await HandleUnknownLearningLanguageAsync(pipelineContext);
            settingsSet = settingsSet && await HandleUnknownKnowsLanguageAsync(pipelineContext);
            
            if (settingsSet)
            {
                pipelineContext.Processed = true;
                IMenuSubMiddleware menuSubMiddleware = _menuSubMiddlewareFactory.Get(TelegramState.OnMainWindow);
                pipelineContext.ResponseMessage = pipelineContext.ResponseMessage.AddText("наконец-то этот экран сделан");
                pipelineContext.ResponseMessage.Html = true;
                await menuSubMiddleware.ChangeState(pipelineContext);
            }
        }

        private async Task<bool> HandleUnknownLanguageAsync(TelegramPipelineContext pipelineContext)
        {
            if (pipelineContext.User.Language == Language.Unknown)
            {
                if (pipelineContext.User.State == TelegramState.AwaitingForLanguage)
                {
                    pipelineContext.Intent = Intent.FirstLanguageSet;
                    if (_telegramButtonsHelper.TryGetLang(pipelineContext.RequestMessage.Text, out var language))
                    {
                        await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateLanguage(language));
                    }
                    else
                    {
                        pipelineContext.IntentNotHandled = true;
                        pipelineContext.ResponseMessage = CreateLangRequest();
                        return false;
                    }
                }
                else
                {
                    pipelineContext.Intent = Intent.Initial;
                    pipelineContext.Processed = true;
                    await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateState(TelegramState.AwaitingForLanguage));
                    pipelineContext.ResponseMessage = CreateLangRequest();
                    return false;
                }
            }
            return true;
        }
        private async Task<bool> HandleUnknownLearningLanguageAsync(TelegramPipelineContext pipelineContext)
        {
            if (pipelineContext.User.LearningLanguage == null)
            {
                if (pipelineContext.User.State == TelegramState.AwaitingLearningLanguage)
                {
                    pipelineContext.Intent = Intent.FirstLearningLanguageSet;
                    if (pipelineContext.RequestMessage.Text == _botLocalization.BackBtnLabel(pipelineContext.User.Language))
                    {
                        pipelineContext.Intent = Intent.FirstLanguageSet;
                        await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateLanguage(Language.Unknown));
                        await HandleUnknownLanguageAsync(pipelineContext);
                        return false;
                    }

                    if (_telegramButtonsHelper.TryCourseLang(pipelineContext.RequestMessage.Text, out var language))
                    {
                        await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateLearningLanguage(language));
                    }
                    else
                    {
                        pipelineContext.IntentNotHandled = true;
                        pipelineContext.ResponseMessage = CreateLearningLangRequest(pipelineContext.User.Language);
                        return false;
                    }
                }
                else
                {
                    await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateState(TelegramState.AwaitingLearningLanguage));
                    pipelineContext.ResponseMessage = CreateLearningLangRequest(pipelineContext.User.Language);
                    return false;
                }
            }
            return true;
        }
        
        private async Task<bool> HandleUnknownKnowsLanguageAsync(TelegramPipelineContext pipelineContext)
        {
            if (pipelineContext.User.LearningLanguage == null)
            {
                if (pipelineContext.User.State == TelegramState.AwaitingKnowsLanguage)
                {
                    pipelineContext.Intent = Intent.FirstKnowsLanguageSet;
                    if (pipelineContext.RequestMessage.Text == _botLocalization.BackBtnLabel(pipelineContext.User.Language))
                    {
                        pipelineContext.Intent = Intent.FirstLearningLanguageSet;
                        await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateLearningLanguage(null));
                        await HandleUnknownLearningLanguageAsync(pipelineContext);
                        return false;
                    }

                    if (_telegramButtonsHelper.TryCourseLang(pipelineContext.RequestMessage.Text, out var language))
                    {
                        await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateKnowsLanguage(language));
                    }
                    else
                    {
                        pipelineContext.IntentNotHandled = true;
                        pipelineContext.ResponseMessage = CreateKnowsLangRequest(pipelineContext.User.Language, pipelineContext.User.LearningLanguage);
                        return false;
                    }
                }
                else
                {
                    await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateState(TelegramState.AwaitingLearningLanguage));
                    pipelineContext.ResponseMessage = CreateKnowsLangRequest(pipelineContext.User.Language, pipelineContext.User.LearningLanguage);
                    return false;
                }
            }
            return true;
        }

        private TelegramResponseMessage CreateLangRequest()
        {
            return new TelegramResponseMessage
            {
                Text = _botLocalization.ChooseLanguage(Language.Unknown),
                ReplyButtons = _telegramButtonsHelper.ForceLanguageButtons
            };
        }
        private TelegramResponseMessage CreateLearningLangRequest(Language language)
        {
            return new TelegramResponseMessage
            {
                Text = _botLocalization.ChooseLearningLanguage(Language.Unknown),
                ReplyButtons = _telegramButtonsHelper.ForceLearningLanguageButtons(language)
            };
        }
        private TelegramResponseMessage CreateKnowsLangRequest(Language language, string learningLanguage)
        {
            return new TelegramResponseMessage
            {
                Text = _botLocalization.ChooseKnowsLanguage(Language.Unknown),
                ReplyButtons = _telegramButtonsHelper.ForceKnowsLanguageButtons(language, learningLanguage)
            };
        }
    }
}
