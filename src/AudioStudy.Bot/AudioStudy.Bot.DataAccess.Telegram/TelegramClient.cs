using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model.Telegram;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace AudioStudy.Bot.DataAccess.Telegram
{
    public class TelegramClient : ITelegramClient
    {
        private readonly TelegramOptions _config;
        private readonly TelegramBotClient _telegramBotClient;

        public TelegramClient(IOptions<TelegramOptions> config)
        {
            _config = config.Value;
            _telegramBotClient = new TelegramBotClient(_config.Token);
        }

        public async Task<IReadOnlyList<TelegramRequestMessage>> GetUpdatesAsync(int offset, int limit,
            CancellationToken cancellationToken)
        {
            var result =
                await _telegramBotClient.GetUpdatesAsync(offset, limit, _config.PollingTimeout.Seconds, new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                }, cancellationToken);
            return result.Select(x =>
            {
                var chat = x.Message?.Chat ?? x.CallbackQuery?.Message?.Chat;
                return new TelegramRequestMessage
                {
                    Id = x.Id,
                    Text = x.Message?.Text,
                    ChatId = chat?.Id ?? default,
                    ChatType = GetTelegramChatType(chat?.Type),
                    CallbackMessageId = x.CallbackQuery?.Message?.MessageId,
                    CallbackQueryId = x.CallbackQuery?.Id,
                    CallbackData = x.CallbackQuery?.Data
                };
            }).ToList();
        }

        public async Task SendAsync(TelegramResponseMessage message)
        {
            IReplyMarkup replyMarkup;
            GetMarkUp(message, out replyMarkup);
            if (message.CallbackMessageId != null)
            {
                if (string.IsNullOrWhiteSpace(message.FileId))
                {
                    await _telegramBotClient.EditMessageCaptionAsync(message.ChatId, message.CallbackMessageId.Value, message.Text, replyMarkup: replyMarkup is InlineKeyboardMarkup markup ? markup : null, parseMode: message.Html ? ParseMode.Html : ParseMode.Default);
                    return;
                }
                else
                {
                    await
                        _telegramBotClient.EditMessageTextAsync(message.ChatId, message.CallbackMessageId.Value,
                            message.Text, disableWebPagePreview: true,
                            replyMarkup: replyMarkup is InlineKeyboardMarkup markup ? markup : null,
                            parseMode: message.Html ? ParseMode.Html : ParseMode.Default);
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(message.FileId))
            {
                await _telegramBotClient.SendAudioAsync(message.ChatId, new InputOnlineFile(message.FileId), message.Text, replyMarkup: replyMarkup, parseMode: message.Html ? ParseMode.Html : ParseMode.Default);
                return;
            }
            await _telegramBotClient.SendTextMessageAsync(message.ChatId, message.Text, replyMarkup: replyMarkup, parseMode: message.Html ? ParseMode.Html : ParseMode.Default);
        }

        public async Task AnswerCallbackQuery(string callBackQueryId)
        {
            await _telegramBotClient.AnswerCallbackQueryAsync(callBackQueryId);
        }

        private static TelegramChatType GetTelegramChatType(ChatType? chatType)
        {
            return chatType switch
            {
                ChatType.Channel => TelegramChatType.Channel,
                ChatType.Group => TelegramChatType.Group,
                ChatType.Private => TelegramChatType.Private,
                ChatType.Supergroup => TelegramChatType.SuperGroup,
                _ => TelegramChatType.Unknown
            };
        }
        
        private static void GetMarkUp(TelegramResponseMessage message, out IReplyMarkup replyMarkup)
        {
            replyMarkup = null;
            if (message.InlineButtons?.Where(x => x != null).SelectMany(x => x).Any() == true)
            {
                replyMarkup = new InlineKeyboardMarkup(message.InlineButtons.Where(x => x != null)
                    .Select(x => x.Select(xx => InlineKeyboardButton.WithCallbackData(xx.Text, xx.CallbackData))
                        .ToArray()).ToArray());
            }
            else 
            if (message.ReplyButtons?.Where(x => x != null).SelectMany(x => x).Any() == true)
            {
                replyMarkup = new ReplyKeyboardMarkup(message.ReplyButtons.Where(x => x != null).Select(x => x.Select(xx => new KeyboardButton(xx.Text)).ToArray()).ToArray(),
                    resizeKeyboard: true);
            }
            else if (message.ReplyButtons?.Length == 0)
            {
                replyMarkup = new ReplyKeyboardRemove();
            }
        }
    }
}