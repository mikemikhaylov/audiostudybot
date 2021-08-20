namespace AudioStudy.Bot.Domain.Services.Courses
{
    public interface ICourseProvider
    {
        string[] GetCoursesLanguages();
        string[] GetTranslationLanguages(string language);
    }
}