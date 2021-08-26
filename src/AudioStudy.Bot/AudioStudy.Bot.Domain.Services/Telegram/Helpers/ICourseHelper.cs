using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Model.Telegram.CallbackData;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public interface ICourseHelper
    {
        TelegramResponseMessage GetCoursePage(User user, OpenCourseCallbackData data);
        Task<TelegramResponseMessage> StartCourse(User user, StartLearningCallbackData data);
        Task<TelegramResponseMessage> StopCourse(User user, StopLearningCallbackData data);
        Task<TelegramResponseMessage> StartOverFromPageCourse(User user, StartOverFromCoursePageCallbackData data);
        Task<TelegramResponseMessage> ConfirmStopCourse(User user, ConfirmStopLearningCallbackData data);
        Task<TelegramResponseMessage> ConfirmStartOverCourse(User user, ConfirmStartOverFromCoursePageCallbackData data);
        Task<TelegramResponseMessage> GetNextLessonAsync(User user, GetNextLessonCallbackData data);
        Task<TelegramResponseMessage> StartOver(User user, StartOverCallbackData data);
    }
}