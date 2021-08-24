using System.Collections.Generic;
using AudioStudy.Bot.Domain.Model.Courses;

namespace AudioStudy.Bot.Domain.Services.Courses
{
    public interface ICourseProvider
    {
        string[] GetCoursesLanguages();
        string[] GetTranslationLanguages(string language);
        IReadOnlyList<Course> GetCourses(string language, string translationLanguage);
    }
}