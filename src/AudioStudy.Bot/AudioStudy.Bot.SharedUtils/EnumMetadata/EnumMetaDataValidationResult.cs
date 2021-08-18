namespace AudioStudy.Bot.SharedUtils.EnumMetadata {
    public class EnumMetaDataValidationResult {
        public bool Valid { get; set; }
        public string Message { get; set; }
        public EnumMetaDataValidationResult(bool valid, string message = null) {
            Valid = valid;
            Message = message;
        }
    }
}
