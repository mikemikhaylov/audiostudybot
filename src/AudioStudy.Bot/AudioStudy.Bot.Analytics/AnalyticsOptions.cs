namespace AudioStudy.Bot.Analytics
{
    public class AnalyticsOptions
    {
        public string Environment { get; set; } = "default";
        public string Token { get; init; }
    }
}