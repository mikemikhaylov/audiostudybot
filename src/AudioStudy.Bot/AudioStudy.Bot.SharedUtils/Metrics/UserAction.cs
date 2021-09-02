using System;

namespace AudioStudy.Bot.SharedUtils.Metrics
{
    public class UserAction
    {
        public UserAction(string userId, Intent analyticsIntent)
        {
            UserId = userId;
            Intent = analyticsIntent;
        }
        public string UserId { get; }
        public Intent Intent { get; }
        public bool NotHandled { get; set; }
    }
}
