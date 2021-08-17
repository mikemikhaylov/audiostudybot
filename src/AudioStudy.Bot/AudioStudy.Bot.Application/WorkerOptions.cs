using System.ComponentModel.DataAnnotations;

namespace AudioStudy.Bot.Application
{
    public class WorkerOptions
    {
        public int UpdatesBatchSize { get; init; } = 100;
    }
}