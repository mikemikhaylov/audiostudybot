namespace AudioStudy.Bot.Domain.Services.Queue
{
    public class QueueOptions
    {
        public int BoundedCapacity { get; init; }
        public int MaxDegreeOfParallelism { get; init; }
    }
}