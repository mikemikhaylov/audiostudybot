using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Courses;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.SharedUtils.Localization;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public abstract class CardsPagingHelperBase<TData>
    {
        private const string PrevPageFormat = "<< {0} {1}";
        private const string NextPageFormat = "{0} {1} >>";
        private readonly IBotLocalization _botLocalization;

        protected CardsPagingHelperBase(IBotLocalization botLocalization)
        {
            _botLocalization = botLocalization;
        }

        public Task<TelegramResponseMessage> GetPageAsync(User user, int page, int pageSize, TData data)
        {
            var skip = page * pageSize;
            var cards = GetCards(user, skip, pageSize + 1, data);
            string responseText;
            if (cards.Any())
            {
                responseText = _botLocalization.Cards(cards.Take(pageSize).Select(x => (Text: x.Text,
                    Transcription: x.Transcription, Translation: x.Translation, Usage: x.Usage,
                    UsageTranslation: x.UsageTranslation)).ToArray());
            }
            else
            {
                responseText = page == 0
                    ? GetNoCardsMessage(user)
                    : _botLocalization.NoCardsOnCurrentPage(user.Language);
            }

            var result = new TelegramResponseMessage
            {
                Text = responseText
            };
            TelegramInlineBtn[] pagesBtns = null;

            if (page == 0)
            {
                if (cards.Count > pageSize)
                {
                    pagesBtns = new[]
                    {
                        new TelegramInlineBtn(
                            string.Format(NextPageFormat, _botLocalization.PageBtnLabel(user.Language), 2),
                            GetOpenPageData(user, 1, pageSize, data))
                    };
                }
            }
            else
            {
                if (cards.Count > pageSize)
                {
                    pagesBtns = new[]
                    {
                        new TelegramInlineBtn(
                            string.Format(PrevPageFormat, _botLocalization.PageBtnLabel(user.Language), page),
                            GetOpenPageData(user, page - 1, pageSize, data)),
                        new TelegramInlineBtn(
                            string.Format(NextPageFormat, _botLocalization.PageBtnLabel(user.Language), page + 2),
                            GetOpenPageData(user, page + 1, pageSize, data))
                    };
                }
                else
                {
                    pagesBtns = new[]
                    {
                        new TelegramInlineBtn(
                            string.Format(PrevPageFormat, _botLocalization.PageBtnLabel(user.Language), page),
                            GetOpenPageData(user, page - 1, pageSize, data))
                    };
                }
            }

            var additionalTop = GetAdditionalTopButtons(user, page, pageSize, data);
            var additionalBottom = GetAdditionalBottomButtons(user, page, pageSize, data);
            var buttons = new[]
                    {additionalTop, pagesBtns == null ? null : new[] {pagesBtns}, additionalBottom}
                .Where(x => x != null).SelectMany(x => x).ToArray();
            if (buttons.Any())
            {
                result.InlineButtons = buttons;
            }

            result.FileId = GetFile(user, data);
            if (!string.IsNullOrWhiteSpace(result.FileId))
            {
                result.IsCaption = true;
            }

            return Task.FromResult(result);
        }
        
        protected abstract IReadOnlyList<Card> GetCards(User user, int skip, int take, TData data);
        protected abstract string GetFile(User user, TData data);
        protected abstract string GetNoCardsMessage(User user);
        protected abstract TelegramInlineBtn[][] GetAdditionalTopButtons(User user, int page, int pageSize, TData data);
        protected abstract TelegramInlineBtn[][] GetAdditionalBottomButtons(User user, int page, int pageSize, TData data);
        protected abstract string GetOpenPageData(User user, int page, int pageSize, TData data);
    }
}