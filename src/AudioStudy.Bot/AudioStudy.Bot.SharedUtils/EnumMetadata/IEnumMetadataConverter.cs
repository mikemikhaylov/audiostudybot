namespace AudioStudy.Bot.SharedUtils.EnumMetadata {
    public interface IEnumMetadataConverter<in TMetadataAttribute, out TMetadata> {
        TMetadata Convert(TMetadataAttribute metadataAttribute);
    }
}
