namespace AudioStudy.Bot.SharedUtils.Metrics
{
    public interface IAnalyticsQueue
    {
        void SendUserAction(UserAction userAction);
    }
}
