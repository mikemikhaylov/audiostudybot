namespace AudioStudy.Bot.Domain.Model.Courses
{
    public class Lesson
    {
        public long FileId { get; set; }
        public Card[] Cards { get; set; }
    }
}