using System;
using System.Threading.Tasks;

namespace AudioStudy.Bot.SharedUtils.Queue
{
    public interface IUpdatesQueueSubscriber<out T>
    {
        void Subscribe(Func<T, Task> handler);
        Task UnsubscribeAsync();
    }
}