using System;
using System.Threading;
using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services.Telegram;
using AudioStudy.Bot.SharedUtils.Queue;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AudioStudy.Bot.Application
{
    public class TelegramPipelineHostedService : IHostedService
    {
        private readonly IUpdatesQueueSubscriber<TelegramRequestMessage> _subscriber;
        private readonly ILogger<TelegramPipelineHostedService> _logger;
        private readonly ITelegramMessagePipeline _telegramMessagePipeline;

        public TelegramPipelineHostedService(
            IUpdatesQueueSubscriber<TelegramRequestMessage> subscriber,
            ILogger<TelegramPipelineHostedService> logger,
            ITelegramMessagePipeline telegramMessagePipeline)
        {
            _subscriber = subscriber;
            _logger = logger;
            _telegramMessagePipeline = telegramMessagePipeline;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(update => _telegramMessagePipeline.HandleMessageAsync(update));
            _logger.LogInformation("{serviceName} started at: {time}", nameof(UpdatesGetterHostedService),
                DateTimeOffset.UtcNow);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _subscriber.UnsubscribeAsync();
        }
    }
}