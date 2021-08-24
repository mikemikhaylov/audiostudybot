using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Telegram;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public interface IFullCourseListPagingHelper
    {
        Task<TelegramResponseMessage> GetFirstPageAsync(User user);
        Task<TelegramResponseMessage> GetPageAsync(User user, int page, int pageSize);
    }
}