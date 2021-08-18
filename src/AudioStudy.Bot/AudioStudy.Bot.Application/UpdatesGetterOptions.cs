using System.ComponentModel.DataAnnotations;

namespace AudioStudy.Bot.Application
{
    public class UpdatesGetterOptions
    {
        public int UpdatesBatchSize { get; init; } = 100;
    }
}