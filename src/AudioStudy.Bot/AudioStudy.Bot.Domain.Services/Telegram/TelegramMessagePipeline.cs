using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;

namespace AudioStudy.Bot.Domain.Services.Telegram
{
    public class TelegramMessagePipeline : ITelegramMessagePipeline
    {
        private readonly IUserService _userService;

        public TelegramMessagePipeline(IUserService userService)
        {
            _userService = userService;
        }
        
        public async Task HandleMessageAsync(TelegramRequestMessage requestMessage)
        {
            var user = await _userService.GetOrCreateAsync(requestMessage.ChatId);
            var context = new TelegramPipelineContext(requestMessage, user);
        }
    }
}