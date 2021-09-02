using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using AudioStudy.Bot.SharedUtils.Metrics;
using Microsoft.Extensions.Options;
using Mixpanel;

namespace AudioStudy.Bot.Analytics
{
    public class AnalyticsQueue: IAnalyticsQueue
    {
        private readonly IOptions<AnalyticsOptions> _config;
        private static bool _started;
        private static readonly object Lock = new();
        private static Task _sendingTask;
        private const int MaxQueueSize = 1000;
        private static readonly ConcurrentQueue<UserAction> Queue = new();

        public AnalyticsQueue(IOptions<AnalyticsOptions> config)
        {
            _config = config;
        }

        public void SendUserAction(UserAction userAction)
        {
            if (!_started)
            {
                lock (Lock)
                {
                    if (!_started)
                    {
                        _started = true;
                        _sendingTask = StartSending(_config.Value.Token, _config.Value.Environment);
                    }
                }
            }
            if (Queue.Count > MaxQueueSize)
            {
                return;
            }
            Queue.Enqueue(userAction);
        }

        private static Task StartSending(string token, string environment)
        {
            return Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await Task.Delay(5000);
                    
                    while (Queue.TryDequeue(out var userAction))
                    {
                        var mc = new MixpanelClient(token);
                        try
                        {
                            await mc.TrackAsync(
                                userAction.Intent.ToString(),
                                userAction.UserId,
                                new
                                {
                                    not_handled = userAction.NotHandled,
                                    environment
                                }
                            );
                        }
                        catch
                        {
                            //ok
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}