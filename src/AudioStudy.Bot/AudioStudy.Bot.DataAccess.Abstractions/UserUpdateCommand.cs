using System;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.SharedUtils.Localization.Enums;

namespace AudioStudy.Bot.DataAccess.Abstractions
{
    public class UserUpdateCommand
    {
        public FieldUpdateCommand<Language> Language { get; set; }
        public FieldUpdateCommand<string> LearningLanguage { get; set; }
        public FieldUpdateCommand<string> KnowsLanguage { get; set; }
        public FieldUpdateCommand<TelegramState> State { get; set; }
        public FieldUpdateCommand<DateTime?> RatingDate { get; set; }

        public static class Factory
        {
            public static UserUpdateCommand UpdateLanguage(Language language)
            {
                return new UserUpdateCommand()
                {
                    Language = new FieldUpdateCommand<Language>(language)
                };
            }
            public static UserUpdateCommand UpdateState(TelegramState state)
            {
                return new UserUpdateCommand()
                {
                    State = new FieldUpdateCommand<TelegramState>(state)
                };
            }
            public static UserUpdateCommand UpdateLearningLanguage(string language)
            {
                return new UserUpdateCommand()
                {
                    LearningLanguage = new FieldUpdateCommand<string>(language)
                };
            }
            public static UserUpdateCommand UpdateKnowsLanguage(string language)
            {
                return new UserUpdateCommand()
                {
                    KnowsLanguage = new FieldUpdateCommand<string>(language)
                };
            }
        }
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