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
            if (!(pipelineContext.User.Language == Language.Unknown
                  || string.IsNullOrWhiteSpace(pipelineContext.User.LearningLanguage)
                  || string.IsNullOrWhiteSpace(pipelineContext.User.KnowsLanguage)))
            {
                return;
            }
            pipelineContext.Processed = true;
            var missingSettings = await HandleUnknownLanguageAsync(pipelineContext);
            missingSettings = missingSettings || await HandleUnknownLearningLanguageAsync(pipelineContext);
            missingSettings = missingSettings || await HandleUnknownKnowsLanguageAsync(pipelineContext);
            
            if (!missingSettings)
            {
                IMenuSubMiddleware menuSubMiddleware = _menuSubMiddlewareFactory.Get(TelegramState.OnMainWindow);
                pipelineContext.ResponseMessage = pipelineContext.ResponseMessage
                    .AddText(_botLocalization.EveryThingSetUp(pipelineContext.User.Language))
                    .AddText(_botLocalization.Help(pipelineContext.User.Language));
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
                        return true;
                    }
                }
                else
                {
                    pipelineContext.Intent = Intent.Initial;
                    await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateState(TelegramState.AwaitingForLanguage));
                    pipelineContext.ResponseMessage = CreateLangRequest();
                    return true;
                }
            }
            return false;
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
                        return await HandleUnknownLanguageAsync(pipelineContext);
                    }

                    if (_telegramButtonsHelper.TryCourseLang(pipelineContext.RequestMessage.Text, out var language))
                    {
                        await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateLearningLanguage(language));
                    }
                    else
                    {
                        pipelineContext.IntentNotHandled = true;
                        pipelineContext.ResponseMessage = CreateLearningLangRequest(pipelineContext.User.Language);
                        return true;
                    }
                }
                else
                {
                    await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateState(TelegramState.AwaitingLearningLanguage));
                    pipelineContext.ResponseMessage = CreateLearningLangRequest(pipelineContext.User.Language);
                    return true;
                }
            }
            return false;
        }
        
        private async Task<bool> HandleUnknownKnowsLanguageAsync(TelegramPipelineContext pipelineContext)
        {
            if (pipelineContext.User.KnowsLanguage == null)
            {
                if (pipelineContext.User.State == TelegramState.AwaitingKnowsLanguage)
                {
                    pipelineContext.Intent = Intent.FirstKnowsLanguageSet;
                    if (pipelineContext.RequestMessage.Text == _botLocalization.BackBtnLabel(pipelineContext.User.Language))
                    {
                        pipelineContext.Intent = Intent.FirstLearningLanguageSet;
                        await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateLearningLanguage(null));
                        return await HandleUnknownLearningLanguageAsync(pipelineContext);
                    }

                    if (_telegramButtonsHelper.TryCourseLang(pipelineContext.RequestMessage.Text, out var language))
                    {
                        await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateKnowsLanguage(language));
                    }
                    else
                    {
                        pipelineContext.IntentNotHandled = true;
                        pipelineContext.ResponseMessage = CreateKnowsLangRequest(pipelineContext.User.Language, pipelineContext.User.LearningLanguage);
                        return true;
                    }
                }
                else
                {
                    await _userService.UpdateAsync(pipelineContext.User, UserUpdateCommand.Factory.UpdateState(TelegramState.AwaitingKnowsLanguage));
                    pipelineContext.ResponseMessage = CreateKnowsLangRequest(pipelineContext.User.Language, pipelineContext.User.LearningLanguage);
                    return true;
                }
            }
            return false;
        }

        private TelegramResponseMessage CreateLangRequest()
        {
            return new TelegramResponseMessage
            {
                Text = _botLocalization.ChooseLanguage(),
                ReplyButtons = _telegramButtonsHelper.ForceLanguageButtons
            };
        }
        private TelegramResponseMessage CreateLearningLangRequest(Language language)
        {
            return new TelegramResponseMessage
            {
                Text = _botLocalization.ChooseLearningLanguage(language),
                ReplyButtons = _telegramButtonsHelper.ForceLearningLanguageButtons(language)
            };
        }
        private TelegramResponseMessage CreateKnowsLangRequest(Language language, string learningLanguage)
        {
            return new TelegramResponseMessage
            {
                Text = _botLocalization.ChooseKnowsLanguage(language),
                ReplyButtons = _telegramButtonsHelper.ForceKnowsLanguageButtons(language, learningLanguage)
            };
        }
    }
}
