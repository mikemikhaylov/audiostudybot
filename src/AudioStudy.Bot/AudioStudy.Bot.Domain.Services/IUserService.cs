using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Courses;

namespace AudioStudy.Bot.Domain.Services
{
    public interface IUserService
    {
        Task<User> GetOrCreateAsync(long chatId);
        Task UpdateAsync(User user, UserUpdateCommand command);
        Task StartLearningCourse(User user, Course course);
        Task StopLearningCourse(User user, Course course);
        Task StartOverCourse(User user, Course course);
        Task SetCurrentLesson(User user, Course course, int currentLesson);
    }
}