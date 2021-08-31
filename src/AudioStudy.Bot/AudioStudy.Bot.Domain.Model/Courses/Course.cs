namespace AudioStudy.Bot.Domain.Model.Courses
{
    public class Course
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public int Weight { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string NameTranslation { get; set; }
        public string DescriptionTranslation { get; set; }
        public string Language { get; set; }
        public string TranslationLanguage { get; set; }
        public Card[] Cards { get; set; }
    }
}