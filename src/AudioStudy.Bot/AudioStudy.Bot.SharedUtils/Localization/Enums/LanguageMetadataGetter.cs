using AudioStudy.Bot.SharedUtils.EnumMetadata;

namespace AudioStudy.Bot.SharedUtils.Localization.Enums
{
    public static class LanguageMetadataGetter
    {
        private static readonly EnumMetadataGetter<Language, LanguageMetadataAttribute, LanguageMetadataAttribute> EnumMetadataGetter;
        static LanguageMetadataGetter()
        {
            EnumMetadataGetter = new EnumMetadataGetter<Language, LanguageMetadataAttribute, LanguageMetadataAttribute>(new NoConversionConverter<LanguageMetadataAttribute, LanguageMetadataAttribute>());
        }
        public static LanguageMetadataAttribute GetMetadata(this Language value)
        {
            return EnumMetadataGetter.GetMetadata(value);
        }
    }
}