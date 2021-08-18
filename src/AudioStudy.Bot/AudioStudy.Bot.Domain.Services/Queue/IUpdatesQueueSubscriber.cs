using System;
using System.Threading.Tasks;

namespace AudioStudy.Bot.Domain.Services.Queue
{
    public interface IUpdatesQueueSubscriber<out T>
    {
        void Subscribe(Func<T, Task> handler);
        void Unsubscribe();
    }
}