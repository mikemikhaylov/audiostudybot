using System;
using System.Threading;
using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services.Abstractions;

namespace AudioStudy.Bot.Domain.Services
{
    public class UpdatesQueueProducer<T>: IUpdatesQueueProducer<T>
    {
        public Task ProduceAsync(T update, CancellationToken cancellationToken)
        {
            var telegramUpdate = update as TelegramUpdate;
            Console.WriteLine("Text: " + telegramUpdate?.Message?.Text ?? "no text");
            return Task.CompletedTask;
        }
    }
}