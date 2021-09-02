using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.SharedUtils.Queue;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AudioStudy.Bot.Application
{
    public class UpdatesGetterHostedService : BackgroundService
    {
        private readonly ILogger<UpdatesGetterHostedService> _logger;
        private readonly IOptions<UpdatesGetterOptions> _config;
        private readonly ITelegramClient _telegramClient;
        private readonly IUpdatesQueuePublisher<TelegramRequestMessage> _updatesQueuePublisher;
        private int _currentOffset;

        public UpdatesGetterHostedService(
            ILogger<UpdatesGetterHostedService> logger,
            IOptions<UpdatesGetterOptions> config,
            ITelegramClient telegramClient,
            IUpdatesQueuePublisher<TelegramRequestMessage> updatesQueuePublisher)
        {
            _logger = logger;
            _config = config;
            _telegramClient = telegramClient;
            _updatesQueuePublisher = updatesQueuePublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{serviceName} started at: {time}", nameof(UpdatesGetterHostedService), DateTimeOffset.UtcNow);
            while (!stoppingToken.IsCancellationRequested)
            {
                IReadOnlyList<TelegramRequestMessage> updates;
                try
                {
                    updates 
                        = await _telegramClient.GetUpdatesAsync(_currentOffset, _config.Value.UpdatesBatchSize, stoppingToken);
                }
                catch (Exception e)
                {
                    if (e is not OperationCanceledException)
                    {
                        _logger.LogError(e, "Error getting updates");
                        await Task.Delay(5000, stoppingToken);
                        continue;
                    }
                    break;
                }
                _logger.LogDebug("Got {count} updates" + DateTime.UtcNow, updates.Count);
                await ProcessUpdates(updates, stoppingToken);
            }
            _logger.LogInformation("{serviceName} finished at: {time}", nameof(UpdatesGetterHostedService), DateTimeOffset.UtcNow);
        }

        private async Task ProcessUpdates(IReadOnlyList<TelegramRequestMessage> updates, CancellationToken cancellationToken)
        {
            if (!updates.Any())
            {
                return;
            }
            foreach (var update in updates)
            {
                await _updatesQueuePublisher.ProduceAsync(update, cancellationToken);
            }
            _currentOffset = updates[^1].Id + 1;
        }
    }
}