using System;
using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.SharedUtils.Localization;
using AudioStudy.Bot.SharedUtils.Metrics;

namespace AudioStudy.Bot.Domain.Services.Telegram.Middlewares
{
    public class CommandExecutorMiddleware : ITelegramPipelineMiddleware
    {
        private readonly IBotLocalization _botLocalization;

        public CommandExecutorMiddleware(
            IBotLocalization botLocalization
            )
        {
            _botLocalization = botLocalization;
        }

        public Task HandleMessageAsync(TelegramPipelineContext pipelineContext)
        {
            string text = pipelineContext.RequestMessage.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                return Task.CompletedTask;
            }
            text = text.TrimStart();
            if (IsCommand(text, CommandConstants.StartCommand))
            {
                //IMenuSubMiddleware sm = MenuSubMiddlewareFactory.Get(TelegramState.OnMainWindow);
                //await sm.ChangeState(pipelineContext);
                pipelineContext.Intent = Intent.StartCommand;
                pipelineContext.Processed = true;
            }
            else if (IsCommand(text, CommandConstants.HelpCommand))
            {
                pipelineContext.ResponseMessage = new TelegramResponseMessage
                {
                    Text = _botLocalization.Help(pipelineContext.User.Language),
                    Html = true
                };
                pipelineContext.Processed = true;
                pipelineContext.Intent = Intent.HelpOpen;
            }
            else if (IsCommand(text, CommandConstants.SettingsCommand))
            {
                //IMenuSubMiddleware sm = MenuSubMiddlewareFactory.Get(TelegramState.OnSettingsWindow);
                //await sm.ChangeState(pipelineContext);
                pipelineContext.Intent = Intent.SettingsScreenOpen;
                pipelineContext.Processed = true;
            }
            return Task.CompletedTask;
        }

        private static bool IsCommand(string text, string command)
        {
            return text.StartsWith(command, StringComparison.OrdinalIgnoreCase);
        }
    }
}
