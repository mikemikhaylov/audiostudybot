namespace AudioStudy.Bot.Domain.Model.Courses
{
    public class CourseLessons
    {
        public string CourseId { get; set; }
        public int CourseVersion { get; set; }
        public Lesson[] Lessons { get; set; }
    }
}