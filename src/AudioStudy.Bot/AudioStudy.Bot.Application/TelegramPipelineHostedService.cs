using System;
using System.Threading;
using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services.Queue;
using Microsoft.Extensions.Hosting;

namespace AudioStudy.Bot.Application
{
    public class TelegramPipelineHostedService : IHostedService
    {
        private readonly IUpdatesQueueSubscriber<TelegramUpdate> _subscriber;

        public TelegramPipelineHostedService(IUpdatesQueueSubscriber<TelegramUpdate> subscriber)
        {
            _subscriber = subscriber;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(async update => { Console.WriteLine("Text: " + update?.Message?.Text); });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscriber.Unsubscribe();
            return Task.CompletedTask;
        }
    }
}