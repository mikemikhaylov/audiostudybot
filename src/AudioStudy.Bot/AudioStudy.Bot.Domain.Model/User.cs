using System;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.SharedUtils.Localization.Enums;

namespace AudioStudy.Bot.Domain.Model
{
    public class User
    {
        public string Id { get; set; }
        public Language Language { get; set; }
        public long ChatId { get; set; }
        public TelegramState State { get; set; }
        public DateTime? RatingDate { get; set; }
    }
}