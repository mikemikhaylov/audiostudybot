using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AudioStudy.Bot.SharedUtils.Localization.LocalizationSource
{
    public class JsonLocalizationSource : ILocalizationSource
    {
        private readonly ILogger _logger;

        public JsonLocalizationSource(ILogger<JsonLocalizationSource> logger)
        {
            _logger = logger;
        }

        private static readonly Lazy<dynamic> Source = new(() =>
        {
            using var stream =
                typeof(JsonLocalizationSource).Assembly.GetManifestResourceStream(
                    "AudioStudy.Bot.Domain.Services.Localization.LocalizationSource.l10n.json");
            if (stream == null)
            {
                throw new Exception("Localization file is missing");
            }

            using var reader = new StreamReader(stream);
            var l10NFile = reader.ReadToEnd();
            return JsonSerializer.Deserialize<dynamic>(l10NFile);
        });

        public string GetKey(string locale, string key)
        {
            try
            {
                return Source.Value[key][locale.ToLowerInvariant()];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in l10n: locale {locale}, key: {key}");
                return key;
            }
        }
    }
}