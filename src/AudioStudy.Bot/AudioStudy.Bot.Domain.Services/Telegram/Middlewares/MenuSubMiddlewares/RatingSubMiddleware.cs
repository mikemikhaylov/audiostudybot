using System;
using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services.Telegram.Helpers;
using AudioStudy.Bot.SharedUtils.Localization;
using AudioStudy.Bot.SharedUtils.Metrics;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares.MenuSubMiddlewares
{
    [MenuSubMiddlewareMetadata(TelegramState = TelegramState.OnRatingWindow)]
    public class RatingSubMiddleware: IMenuSubMiddleware
    {
        private readonly ITelegramButtonsHelper _buttonsHelper;
        private readonly IBotLocalization _botLocalization;
        private readonly IUserService _userService;
        private readonly IMenuSubMiddlewareFactory _menuSubMiddlewareFactory;

        public RatingSubMiddleware(ITelegramButtonsHelper buttonsHelper,
            IBotLocalization botLocalization,
            IUserService userService,
            IMenuSubMiddlewareFactory menuSubMiddlewareFactory)
        {
            _buttonsHelper = buttonsHelper;
            _botLocalization = botLocalization;
            _userService = userService;
            _menuSubMiddlewareFactory = menuSubMiddlewareFactory;
        }

        public async Task ChangeState(TelegramPipelineContext pipelineContext, UserUpdateCommand updateCommand = null)
        {
            pipelineContext.ResponseMessage = pipelineContext.ResponseMessage.AddText(_botLocalization.RatingInstruction(pipelineContext.User.Language));
            pipelineContext.ResponseMessage.ReplyButtons = _buttonsHelper.GetStateButtons(TelegramState.OnRatingWindow, pipelineContext.User);
            updateCommand = updateCommand.Combine((uc, fu) => uc.State = fu, TelegramState.OnRatingWindow);
            await _userService.UpdateAsync(pipelineContext.User, updateCommand);
        }

        public async Task ProcessState(TelegramPipelineContext pipelineContext)
        {
            var currentUser = pipelineContext.User;
            if (pipelineContext.RequestMessage.Text == _botLocalization.HaveRatedBtnLabel(currentUser.Language))
            {
                pipelineContext.ResponseMessage = new TelegramResponseMessage().AddText(_botLocalization.ThanksForRating(currentUser.Language));
                UserUpdateCommand updateCommand = new UserUpdateCommand
                {
                    RatingDate = new FieldUpdateCommand<DateTime?>(null)
                };
                currentUser.RatingDate = null;
                IMenuSubMiddleware sm = _menuSubMiddlewareFactory.Get(TelegramState.OnMainWindow);
                await sm.ChangeState(pipelineContext, updateCommand);
                pipelineContext.Intent = Intent.HaveRated;
            }
            else if (pipelineContext.RequestMessage.Text == _botLocalization.WillRateLaterBtnLabel(currentUser.Language))
            {
                pipelineContext.ResponseMessage = new TelegramResponseMessage().AddText(_botLocalization.WillRateLaterAnswer(currentUser.Language));
                var ratingDate = DateTime.UtcNow.AddDays(3);
                UserUpdateCommand updateCommand = new UserUpdateCommand
                {
                    RatingDate = new FieldUpdateCommand<DateTime?>(ratingDate)
                };
                currentUser.RatingDate = ratingDate;
                IMenuSubMiddleware sm = _menuSubMiddlewareFactory.Get(TelegramState.OnMainWindow);
                await sm.ChangeState(pipelineContext, updateCommand);
                pipelineContext.Intent = Intent.WillRateLater;
            }
            else if (pipelineContext.RequestMessage.Text == _botLocalization.WillNotRateBtnLabel(currentUser.Language))
            {
                pipelineContext.ResponseMessage = new TelegramResponseMessage().AddText(_botLocalization.WillNotRateAnswer(currentUser.Language));
                UserUpdateCommand updateCommand = new UserUpdateCommand
                {
                    RatingDate = new FieldUpdateCommand<DateTime?>(null)
                };
                currentUser.RatingDate = null;
                IMenuSubMiddleware sm = _menuSubMiddlewareFactory.Get(TelegramState.OnMainWindow);
                await sm.ChangeState(pipelineContext, updateCommand);
                pipelineContext.Intent = Intent.WillNotRate;
            }
            else
            {
                pipelineContext.ResponseMessage = pipelineContext.ResponseMessage.SetReplyButtons(
                        _buttonsHelper.GetStateButtons(TelegramState.OnRatingWindow, pipelineContext.User))
                    .AddText(_botLocalization.UnknownCommand(pipelineContext.User.Language));
            }
            pipelineContext.Processed = true;
        }
    }
}