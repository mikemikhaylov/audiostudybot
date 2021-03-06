using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Model.Telegram.CallbackData;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public interface IFilterHelper
    {
        TelegramResponseMessage GetLearningLanguageFilter(User user, OpenFilterCallbackData data);
        TelegramResponseMessage GetKnowsLanguageFilter(User user, SetKnowsLanguageCallbackData data);
        Task<TelegramResponseMessage> SetFilter(User user, SetFilterCallbackData data);
    }
}