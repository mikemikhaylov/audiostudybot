using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Model.Telegram.CallbackData;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public interface ILessonCardsPagingHelper
    {
        Task<TelegramResponseMessage> GetPageAsync(User user, OpenLessonCardsPageCallbackData data);
    }
}