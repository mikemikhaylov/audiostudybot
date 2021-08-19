using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model;

namespace AudioStudy.Bot.DataAccess.Abstractions
{
    public interface IUserRepository
    {
        Task<User> GetUserByChatIdAsync(long chatId);
        Task CreateAsync(User user);
        Task UpdateAsync(User user, UserUpdateCommand command);
    }
}