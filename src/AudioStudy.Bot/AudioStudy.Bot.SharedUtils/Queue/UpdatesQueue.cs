using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AudioStudy.Bot.SharedUtils.Queue
{
    public class UpdatesQueue<T> : IUpdatesQueuePublisher<T>, IUpdatesQueueSubscriber<T>
    {
        private readonly IOptions<QueueOptions> _config;
        private readonly ILogger<UpdatesQueue<T>> _logger;
        private readonly BufferBlock<T> _bufferBlock;
        private ActionBlock<T> _subscriptionBlock;

        public UpdatesQueue(IOptions<QueueOptions> config, ILogger<UpdatesQueue<T>> logger)
        {
            _config = config;
            _logger = logger;
            _bufferBlock = new BufferBlock<T>(new DataflowBlockOptions
            {
                BoundedCapacity = config.Value.BoundedCapacity
            });
        }

        public async Task ProduceAsync(T update, CancellationToken cancellationToken)
        {
            await _bufferBlock.SendAsync(update, cancellationToken);
        }

        public void Subscribe(Func<T, Task> handler)
        {
            _subscriptionBlock = new ActionBlock<T>(async item =>
            {
                try
                {
                    await handler(item);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error in handler");
                }
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _config.Value.MaxDegreeOfParallelism,
                BoundedCapacity = _config.Value.BoundedCapacity,
                EnsureOrdered = false
            });
            _bufferBlock.LinkTo(_subscriptionBlock, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });
        }

        public Task UnsubscribeAsync()
        {
            if (_subscriptionBlock == null)
            {
                return Task.CompletedTask;
            }
            _bufferBlock.Complete();
            return _subscriptionBlock.Completion;
        }
    }
}