using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services.Courses;
using AudioStudy.Bot.SharedUtils.Localization;
using AudioStudy.Bot.SharedUtils.Localization.Enums;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public class TelegramButtonsHelper : ITelegramButtonsHelper
    {
        private readonly IBotLocalization _botLocalization;
        private readonly ICourseProvider _courseProvider;

        public TelegramButtonsHelper(
            IBotLocalization botLocalization,
            ICourseProvider courseProvider)
        {
            _botLocalization = botLocalization;
            _courseProvider = courseProvider;
        }

        private static readonly Lazy<Dictionary<string, Language>> LangByBtn = new(() =>
        {
            return
                Enum.GetValues(typeof(Language))
                    .Cast<Language>()
                    .Where(x=>x != Language.Unknown)
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

        private static readonly ConcurrentDictionary<string, string> CourseLanguageByButton =
            new(StringComparer.InvariantCultureIgnoreCase);

        public bool TryCourseLang(string text, out string language)
        {
            return CourseLanguageByButton.TryGetValue(text, out language);
        }

        public TelegramReplyBtn[][] ForceLearningLanguageButtons(Language language) =>
            GetCourseLanguageButtons(language, _courseProvider.GetCoursesLanguages());

        public TelegramReplyBtn[][] ForceKnowsLanguageButtons(Language language, string learningLanguage) =>
            GetCourseLanguageButtons(language, _courseProvider.GetTranslationLanguages(learningLanguage));

        private TelegramReplyBtn[][] GetCourseLanguageButtons(Language language, string[] languages)
        {
            return
                languages.Select(x =>
                    {
                        var text = _botLocalization.CourseLanguage(language, x);
                        if (!CourseLanguageByButton.ContainsKey(x))
                        {
                            CourseLanguageByButton[x] = text;
                        }
                        return text;
                    })
                    .Concat(new[] {_botLocalization.BackBtnLabel(language)})
                    .Select((item, inx) => new {item, inx})
                    .GroupBy(x => x.inx / 2)
                    .Select(g => g.Select(x => x.item))
                    .Select(x => x.Select(xx => new TelegramReplyBtn {Text = xx}).ToArray()).ToArray();
        }

        private static readonly Lazy<TelegramReplyBtn[]> LangBtns = new(() =>
        {
            return Enum.GetValues(typeof(Language))
                .Cast<Language>().Select(x => new TelegramReplyBtn
                {
                    Text = x.GetMetadata().Label
                }).ToArray();
        });

        public TelegramReplyBtn[][] ForceLanguageButtons => new[] {LangBtns.Value};

        public TelegramReplyBtn[][] GetStateButtons(TelegramState telegramState, User user)
        {
            switch (telegramState)
            {
                case TelegramState.OnMainWindow:
                    return
                        user.RatingDate <= DateTime.UtcNow
                            ? MainWindowWithRateButtons(user.Language)
                            : MainWindowButtons(user.Language);
                case TelegramState.AwaitingForLanguage:
                    return LanguageSettingButtons(user.Language);
                case TelegramState.OnRatingWindow:
                    return RatingButtons(user);
                default:
                    throw new NotImplementedException($"Buttons search for {telegramState} state is not implemented");
            }
        }

        private TelegramReplyBtn[][] LanguageSettingButtons(Language language) => new[]
        {
            LangBtns.Value,
            new[]
            {
                new TelegramReplyBtn {Text = _botLocalization.CancelBtnLabel(language)}
            }
        };

        #region main window btns

        private TelegramReplyBtn[][] MainWindowButtons(Language language) => new[]
        {
            new[]
            {
                new TelegramReplyBtn {Text = _botLocalization.LearnBtnLabel(language)},
                new TelegramReplyBtn {Text = _botLocalization.MyCoursesBtnLabel(language)}
            },
            new[]
            {
                new TelegramReplyBtn {Text = _botLocalization.CoursesBtnLabel(language)},
                new TelegramReplyBtn {Text = _botLocalization.SettingsBtnLabel(language)}
            }
        };

        private TelegramReplyBtn[][] MainWindowWithRateButtons(Language language) => new[]
        {
            new[] {new TelegramReplyBtn {Text = _botLocalization.DoYouLikeThisBotBtnLabel(language)}}
        }.Concat(MainWindowButtons(language)).ToArray();

        #endregion

        private TelegramReplyBtn[][] RatingButtons(User user) => new[]
        {
            new[]
            {
                new TelegramReplyBtn {Text = _botLocalization.HaveRatedBtnLabel(user.Language)}
            },
            new[]
            {
                new TelegramReplyBtn {Text = _botLocalization.WillRateLaterBtnLabel(user.Language)}
            },
            new[]
            {
                new TelegramReplyBtn {Text = _botLocalization.WillNotRateBtnLabel(user.Language)}
            }
        };
    }
}