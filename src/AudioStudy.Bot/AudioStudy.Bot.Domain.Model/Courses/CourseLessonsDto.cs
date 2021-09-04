namespace AudioStudy.Bot.Domain.Model.Courses
{
    public class CourseLessonsDto
    {
        public string CourseId { get; set; }
        public int CourseVersion { get; set; }
        public LessonDto[] Lessons { get; set; }
    }
}