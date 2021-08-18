namespace AudioStudy.Bot.SharedUtils.Queue
{
    public class QueueOptions
    {
        public int BoundedCapacity { get; init; }
        public int MaxDegreeOfParallelism { get; init; }
    }
}