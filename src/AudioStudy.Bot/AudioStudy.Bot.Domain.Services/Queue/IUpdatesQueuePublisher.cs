using System.Threading;
using System.Threading.Tasks;

namespace AudioStudy.Bot.Domain.Services.Queue
{
    public interface IUpdatesQueuePublisher<in T>
    {
        Task ProduceAsync(T update, CancellationToken cancellationToken);
    }
}