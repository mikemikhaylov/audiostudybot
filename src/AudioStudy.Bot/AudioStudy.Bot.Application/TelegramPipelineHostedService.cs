using System;
using System.Threading;
using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.SharedUtils.Queue;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AudioStudy.Bot.Application
{
    public class TelegramPipelineHostedService : IHostedService
    {
        private readonly IUpdatesQueueSubscriber<TelegramUpdate> _subscriber;
        private readonly ILogger<TelegramPipelineHostedService> _logger;

        public TelegramPipelineHostedService(
            IUpdatesQueueSubscriber<TelegramUpdate> subscriber,
            ILogger<TelegramPipelineHostedService> logger)
        {
            _subscriber = subscriber;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(async update => { Console.WriteLine("Text: " + update?.Message?.Text); });
            _logger.LogInformation("{serviceName} started at: {time}", nameof(UpdatesGetterHostedService), DateTimeOffset.UtcNow);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _subscriber.UnsubscribeAsync();
        }
    }
}