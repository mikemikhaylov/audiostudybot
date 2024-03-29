﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model.Telegram;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace AudioStudy.Bot.DataAccess.Telegram
{
    public class TelegramClient : ITelegramClient
    {
        private readonly ILogger<TelegramClient> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly TelegramOptions _config;

        public TelegramClient(IOptions<TelegramOptions> config,
            ILogger<TelegramClient> logger,
            IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _config = config.Value;
        }

        public async Task<IReadOnlyList<TelegramRequestMessage>> GetUpdatesAsync(int offset, int limit,
            CancellationToken cancellationToken)
        {
            var result =
                await GetTelegramBotClient().GetUpdatesAsync(offset, limit, (int)_config.PollingTimeout.TotalSeconds, new[]
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
            GetMarkUp(message, out var replyMarkup);
            if (message.CallbackMessageId != null)
            {
                if (!string.IsNullOrWhiteSpace(message.FileId) || message.IsCaption)
                {
                    await GetTelegramBotClient().EditMessageCaptionAsync(message.ChatId, message.CallbackMessageId.Value,
                        SanitizeText(message.Text, _config.CaptionMaxLength),
                        replyMarkup: replyMarkup is InlineKeyboardMarkup markup ? markup : null,
                        parseMode: message.Html ? ParseMode.Html : ParseMode.Default);
                }
                else
                {
                    await
                        GetTelegramBotClient().EditMessageTextAsync(message.ChatId, message.CallbackMessageId.Value,
                            SanitizeText(message.Text, _config.TextMaxLength), disableWebPagePreview: true,
                            replyMarkup: replyMarkup is InlineKeyboardMarkup markup ? markup : null,
                            parseMode: message.Html ? ParseMode.Html : ParseMode.Default);
                }

                return;
            }

            if (!string.IsNullOrWhiteSpace(message.FileId))
            {
                await GetTelegramBotClient().SendAudioAsync(message.ChatId, new InputOnlineFile(message.FileId),
                    SanitizeText(message.Text, _config.CaptionMaxLength), replyMarkup: replyMarkup,
                    parseMode: message.Html ? ParseMode.Html : ParseMode.Default);
                return;
            }

            await GetTelegramBotClient().SendTextMessageAsync(message.ChatId,
                SanitizeText(message.Text, _config.TextMaxLength), replyMarkup: replyMarkup,
                parseMode: message.Html ? ParseMode.Html : ParseMode.Default, disableWebPagePreview: true);
        }

        public async Task AnswerCallbackQuery(string callBackQueryId)
        {
            await GetTelegramBotClient().AnswerCallbackQueryAsync(callBackQueryId);
        }

        private TelegramBotClient GetTelegramBotClient()
        {
            return new TelegramBotClient(_config.Token, _clientFactory.CreateClient());
        }
        private string SanitizeText(string text, int maxLength)
        {
            if (text == null || text.Length <= maxLength)
            {
                return text;
            };
            _logger.LogError($"Message text is greater than {maxLength}: ${text.Substring(0, 50)}");
            return text.Substring(0, maxLength);
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
            else if (message.ReplyButtons?.Where(x => x != null).SelectMany(x => x).Any() == true)
            {
                replyMarkup = new ReplyKeyboardMarkup(
                    message.ReplyButtons.Where(x => x != null)
                        .Select(x => x.Select(xx => new KeyboardButton(xx.Text)).ToArray()).ToArray(),
                    resizeKeyboard: true);
            }
            else if (message.ReplyButtons?.Length == 0)
            {
                replyMarkup = new ReplyKeyboardRemove();
            }
        }
    }
}