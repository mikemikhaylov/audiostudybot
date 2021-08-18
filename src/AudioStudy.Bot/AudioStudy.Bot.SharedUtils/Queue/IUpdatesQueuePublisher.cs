using System.Threading;
using System.Threading.Tasks;

namespace AudioStudy.Bot.SharedUtils.Queue
{
    public interface IUpdatesQueuePublisher<in T>
    {
        Task ProduceAsync(T update, CancellationToken cancellationToken);
    }
}