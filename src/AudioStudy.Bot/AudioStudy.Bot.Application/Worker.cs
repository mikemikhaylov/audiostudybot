using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AudioStudy.Bot.Application
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptions<WorkerOptions> _config;
        private readonly ITelegramClient _telegramClient;
        private readonly IUpdatesQueueProducer<TelegramUpdate> _updatesQueueProducer;
        private int _currentOffset;

        public Worker(
            ILogger<Worker> logger,
            IOptions<WorkerOptions> config,
            ITelegramClient telegramClient,
            IUpdatesQueueProducer<TelegramUpdate> updatesQueueProducer)
        {
            _logger = logger;
            _config = config;
            _telegramClient = telegramClient;
            _updatesQueueProducer = updatesQueueProducer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started at: {time}", DateTimeOffset.UtcNow);
            while (!stoppingToken.IsCancellationRequested)
            {
                var updates 
                    = await _telegramClient.GetUpdatesAsync(_currentOffset, _config.Value.UpdatesBatchSize, stoppingToken);
                _logger.LogDebug("Got {count} updates", updates.Count);
                await ProcessUpdates(updates, stoppingToken);
            }
            _logger.LogInformation("Worker finished at: {time}", DateTimeOffset.UtcNow);
        }

        private async Task ProcessUpdates(IReadOnlyList<TelegramUpdate> updates, CancellationToken cancellationToken)
        {
            if (!updates.Any())
            {
                return;
            }
            foreach (var update in updates)
            {
                await _updatesQueueProducer.ProduceAsync(update, cancellationToken);
            }
            _currentOffset = updates[^1].Id + 1;
        }
    }
}