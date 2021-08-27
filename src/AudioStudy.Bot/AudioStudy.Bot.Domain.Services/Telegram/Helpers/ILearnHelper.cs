using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Model.Telegram.CallbackData;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public interface ILearnHelper
    {
        Task<TelegramResponseMessage> GetFirstPageAsync(User user);
        Task<TelegramResponseMessage> GetPageAsync(User user, OpenPageToStudyCallbackData data);
        Task<TelegramResponseMessage> GetLearnPage(User user);
        Task<TelegramResponseMessage> SetCourseToLearn(User user, SetCourseToLearnCallbackData data);
    }
}