using AudioStudy.Bot.Domain.Model.Courses;

namespace AudioStudy.Bot.Domain.Services.Courses
{
    public interface ILessonProvider
    {
        Lesson[] GetCourseLessons(string courseId, int courseVersion);
        Lesson GetNextLesson(string courseId, int courseVersion, int currentLesson);
    }
}