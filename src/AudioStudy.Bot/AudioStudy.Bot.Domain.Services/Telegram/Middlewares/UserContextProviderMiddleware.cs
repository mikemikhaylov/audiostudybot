using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares
{
    public class UserContextProviderMiddleware : ITelegramPipelineMiddleware
    {
        private readonly IUserService _userService;

        public UserContextProviderMiddleware(IUserService userService)
        {
            _userService = userService;
        }
        public async Task HandleMessageAsync(TelegramPipelineContext pipelineContext)
        {
            var user = await _userService.GetOrCreateAsync(pipelineContext.RequestMessage.ChatId);
            pipelineContext.User = user;
        }
    }
}