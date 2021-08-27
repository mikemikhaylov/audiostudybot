using System.Collections.Generic;

namespace AudioStudy.Bot.Domain.Model.Telegram.CallbackData
{
    public class OpenLearnPageCallbackData : TelegramCallbackDataBase
    {
        public override string ToString()
        {
            return ToString(TelegramInlineBtnType.OpenLearnPage);
        }
    }
}