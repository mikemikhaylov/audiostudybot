using System;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.SharedUtils.Localization.Enums;

namespace AudioStudy.Bot.DataAccess.Abstractions
{
    public class UserUpdateCommand
    {
        public FieldUpdateCommand<Language> Language { get; set; }
        public FieldUpdateCommand<TelegramState> State { get; set; }
        public FieldUpdateCommand<DateTime?> RatingDate { get; set; }
    }
    
    public static class UserUpdateCommandExtensions
    {
        public static UserUpdateCommand Combine<T>(this UserUpdateCommand updateCommand,
            Action<UserUpdateCommand, FieldUpdateCommand<T>> updater, T fieldValue)
        {
            updateCommand ??= new UserUpdateCommand();
            updater(updateCommand, new FieldUpdateCommand<T>(fieldValue));
            return updateCommand;
        }
    }
}