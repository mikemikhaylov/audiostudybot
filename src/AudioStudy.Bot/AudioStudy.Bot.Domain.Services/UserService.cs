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
    }
}