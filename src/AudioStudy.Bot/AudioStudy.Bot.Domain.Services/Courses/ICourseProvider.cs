using System.Collections.Generic;
using AudioStudy.Bot.Domain.Model.Courses;
using AudioStudy.Bot.SharedUtils.Localization.Enums;

namespace AudioStudy.Bot.Domain.Services.Courses
{
    public interface ICourseProvider
    {
        string[] GetCoursesLanguages();
        string[] GetTranslationLanguages(string language);
        IReadOnlyList<Course> GetCourses(string language, string translationLanguage);
        string GetCourseName(Language language, Course course);
        string GetCourseDescription(Language language, Course course);
    }
}