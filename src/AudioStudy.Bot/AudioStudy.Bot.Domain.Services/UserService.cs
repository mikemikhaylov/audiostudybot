using System;
using System.Linq;
using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Courses;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.SharedUtils.Localization.Enums;

namespace AudioStudy.Bot.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetOrCreateAsync(long chatId)
        {
            var user = await _userRepository.GetUserByChatIdAsync(chatId);
            if (user != null)
            {
                return user;
            }

            user = new User
            {
                Language = Language.Unknown,
                RatingDate = DateTime.UtcNow.AddDays(7),
                ChatId = chatId,
                State = TelegramState.Unknown
            };
            try
            {
                await _userRepository.CreateAsync(user);
            }
            catch (Exception)
            {
                user = await _userRepository.GetUserByChatIdAsync(chatId);
                if (user != null)
                {
                    return user;
                }

                throw;
            }

            return user;
        }

        public Task UpdateAsync(User user, UserUpdateCommand command)
        {
            return _userRepository.UpdateAsync(user, command);
        }

        public async Task StartLearningCourse(User user, Course course)
        {
            var courses = GetUserCourses(user);
            var index = Array.FindIndex(courses, x => x.Id == course.Id);
            if (index > -1)
            {
                if (index == 0)
                {
                    return;
                }

                await SetUserCourses(user,
                    new[] {courses[index]}.Concat(courses.Where(x => x.Id != course.Id)).ToArray());
            }
            else
            {
                await SetUserCourses(user, new[] {CreateUserCourse(course)}.Concat(courses).ToArray());
            }
        }

        public async Task StopLearningCourse(User user, Course course)
        {
            var courses = GetUserCourses(user);
            if (courses.Any(x => x.Id == course.Id))
            {
                await SetUserCourses(user, courses.Where(x => x.Id != course.Id).ToArray());
            }
        }

        public async Task StartOverCourse(User user, Course course)
        {
            var courses = GetUserCourses(user);
            await SetUserCourses(user,
                new[] {CreateUserCourse(course)}.Concat(courses.Where(x => x.Id != course.Id)).ToArray());
        }

        private async Task SetUserCourses(User user, UserCourse[] userCourses)
        {
            await UpdateAsync(user, UserUpdateCommand.Factory.UpdateCourses(userCourses));
        }

        private UserCourse CreateUserCourse(Course course) => new UserCourse
        {
            Id = course.Id,
            LastLesson = -1,
            Version = course.Version
        };

        private UserCourse[] GetUserCourses(User user)
        {
            if (user.Courses == null)
            {
                return Array.Empty<UserCourse>();
            }

            if (!user.Courses.Any())
            {
                return user.Courses;
            }

            return user.Courses.GroupBy(x => x.Id).Select(x => x.First()).ToArray();
        }
    }
}