namespace AudioStudy.Bot.SharedUtils.EnumMetadata {
    public class NoConversionConverter<TMetadataAttribute, TMetadata> : IEnumMetadataConverter<TMetadataAttribute, TMetadata> where TMetadataAttribute : TMetadata {
        public TMetadata Convert(TMetadataAttribute metadataAttribute) {
            return metadataAttribute;
        }
    }
}
