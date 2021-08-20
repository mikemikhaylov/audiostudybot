using System;
using System.Collections.Generic;
using System.Linq;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.SharedUtils.Localization;
using AudioStudy.Bot.SharedUtils.Localization.Enums;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public class TelegramButtonsHelper : ITelegramButtonsHelper
    {
        private readonly IBotLocalization _botLocalization;

        public TelegramButtonsHelper(IBotLocalization botLocalization)
        {
            _botLocalization = botLocalization;
        }

        private static readonly Lazy<Dictionary<string, Language>> LangByBtn = new Lazy<Dictionary<string, Language>>(() =>
        {
            return
                Enum.GetValues(typeof(Language))
                    .Cast<Language>()
                    .ToDictionary(x => x.GetMetadata().Label, x => x);
        });

        public bool TryGetLang(string text, out Language language)
        {
            if (text == null)
            {
                language = Language.Unknown;
                return false;
            }
            return LangByBtn.Value.TryGetValue(text.Trim(), out language);
        }

        private static readonly Lazy<TelegramReplyBtn[]> LangBtns = new(() =>
        {
            return Enum.GetValues(typeof(Language))
                .Cast<Language>().Select(x => new TelegramReplyBtn
                {
                    Text = x.GetMetadata().Label
                }).ToArray();

        });

        public TelegramReplyBtn[][] ForceLanguageButtons => new[] { LangBtns.Value };

        private TelegramReplyBtn[][] LanguageSettingButtons => new[] {
            LangBtns.Value,
            new[]
        {
            new TelegramReplyBtn { Text = _botLocalization.CancelBtnLabel }
        } };

        public TelegramReplyBtn[][] GetStateButtons(TelegramState telegramState, User user)
        {
            switch (telegramState)
            {
                case TelegramState.OnMainWindow:
                    return
                        user.RatingDate <= DateTime.UtcNow
                        ? MainWindowWithRateButtons
                        : MainWindowButtons;
                case TelegramState.AwaitingForLanguage:
                    return LanguageSettingButtons;
                case TelegramState.OnRatingWindow:
                    return RatingButtons(user);
                default: throw new NotImplementedException($"Buttons search for {telegramState} state is not implemented");
            }
        }

        #region main window btns
        private TelegramReplyBtn[][] MainWindowButtons => new[]
        {
            new[] {new TelegramReplyBtn {Text = _messengerLocalization.}, new TelegramReplyBtn {Text = _messengerLocalization.}},
            new[] {new TelegramReplyBtn {Text = _messengerLocalization.SettingsBtnLabel}, new TelegramReplyBtn { Text = _messengerLocalization. } }
        };

        private TelegramReplyBtn[][] MainWindowWithRateButtons => new[]
        {
            new[] {new TelegramReplyBtn {Text = _messengerLocalization.DoYouLikeThisBotBtnLabel}},
            new[] {new TelegramReplyBtn {Text = _messengerLocalization.}, new TelegramReplyBtn {Text = _messengerLocalization.}},
            new[] {new TelegramReplyBtn {Text = _messengerLocalization.SettingsBtnLabel}, new TelegramReplyBtn { Text = _messengerLocalization. } }
        };

        #endregion

        private TelegramReplyBtn[][] RatingButtons(User user) => new[] { new[]
        {
            new TelegramReplyBtn { Text = _botLocalization.HaveRatedBtnLabel(user.Language) }
        },
        new[]
        {
            new TelegramReplyBtn { Text = _botLocalization.WillRateLaterBtnLabel(user.Language) }
        },
        new[]
        {
            new TelegramReplyBtn { Text = _botLocalization.WillNotRateBtnLabel(user.Language) }
        }};
    }
}