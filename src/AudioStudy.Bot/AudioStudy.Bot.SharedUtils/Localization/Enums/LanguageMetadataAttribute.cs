using AudioStudy.Bot.SharedUtils.EnumMetadata;

namespace AudioStudy.Bot.SharedUtils.Localization.Enums
{
    public class LanguageMetadataAttribute: EnumMetadataBaseAttribute
    {
        public string Label { get; set; }
        public string Short { get; set; }
    }
}