namespace AudioStudy.Bot.Domain.Model.Courses
{
    public class Card
    {
        public string Text { get; set; }
        public string Transcription { get; set; }
        public string Translation { get; set; }
        public string Usage { get; set; }
        public string UsageTranslation { get; set; }
        
        /// <summary>
        /// it is a lame design, but for now this field is only used in lesson cards
        /// </summary>
        public bool IsNew { get; set; }

        public void SanitizeCourseCard()
        {
            IsNew = false;
        }
    }
}