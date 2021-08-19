using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model;

namespace AudioStudy.Bot.Domain.Services
{
    public interface IUserService
    {
        Task<User> GetOrCreateAsync(long chatId);
        Task UpdateAsync(User user, UserUpdateCommand command);
    }
}