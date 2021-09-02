using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AudioStudy.Bot.Courses;
using AudioStudy.Bot.Domain.Model.Courses;
using AudioStudy.Bot.SharedUtils.Localization.Enums;

namespace AudioStudy.Bot.Domain.Services.Courses
{
    public class CourseProvider : ICourseProvider
    {
        private static readonly Lazy<Course[]> Courses = new(GetAllCourses);

        private static readonly Lazy<Dictionary<string, Course>> CoursesById = new(() =>
        {
            return Courses.Value.ToDictionary(x => x.Id, x => x);
        });

        private static readonly Lazy<string[]> CoursesLanguages = new(() => SortLanguages(Courses.Value.Select(x =>
        x.Language).Distinct()));

        private static readonly Lazy<Dictionary<string, string[]>> TranslationLanguages = new(() =>
        {
            var result = new Dictionary<string, HashSet<string>>();
            foreach (var languagePairs in Courses.Value.SelectMany(x =>
            {
                return new[] {new[] {x.Language, x.TranslationLanguage}};
            }))
            {
                if (!result.ContainsKey(languagePairs[0]))
                {
                    result[languagePairs[0]] = new HashSet<string>();
                }

                result[languagePairs[0]].Add(languagePairs[1]);
            }

            return result.ToDictionary(x => x.Key, x => SortLanguages(x.Value));
        });

        public string[] GetCoursesLanguages()
        {
            return CoursesLanguages.Value;
        }

        public string[] GetTranslationLanguages(string language)
        {
            if (TranslationLanguages.Value.TryGetValue(language, out var languages))
            {
                return languages;
            }

            return Array.Empty<string>();
        }

        public IReadOnlyList<Course> GetCourses(string language, string translationLanguage)
        {
            return Courses.Value.Where(x => x.Language == language && x.TranslationLanguage == translationLanguage)
                .ToList();
        }

        public string GetCourseName(Language language, Course course)
        {
            var languageShort = language.GetMetadata().Short;
            if (string.Equals(course.TranslationLanguage, languageShort, StringComparison.InvariantCultureIgnoreCase))
            {
                return course.NameTranslation ?? course.Name;
            }

            return course.Name;
        }

        public string GetCourseDescription(Language language, Course course)
        {
            var languageShort = language.GetMetadata().Short;
            if (string.Equals(course.TranslationLanguage, languageShort, StringComparison.InvariantCultureIgnoreCase))
            {
                return course.DescriptionTranslation ?? course.Description;
            }

            return course.Description;
        }

        public Course GetCourse(string courseId)
        {
            CoursesById.Value.TryGetValue(courseId, out var course);
            return course;
        }

        public void Load()
        {
            var loaded = Courses.Value;
            var tmp = CoursesLanguages.Value;
            var tmp2 = TranslationLanguages.Value;
            var tmp3 = CoursesById.Value;
        }

        private static Course[] GetAllCourses()
        {
            var result = new List<Course>();
            var assembly = typeof(CoursesAnchor).Assembly;
            var resources = assembly.GetManifestResourceNames().Where(x =>
                x.StartsWith($"{assembly.GetName().Name}.courses.") && x.EndsWith(".json"));
            foreach (var resource in resources)
            {
                using var stream = typeof(CoursesAnchor).Assembly.GetManifestResourceStream(resource);
                using var reader = new StreamReader(stream!);
                var settings = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                settings.Converters.Add(new JsonStringEnumConverter());
                var course = JsonSerializer.Deserialize<Course>(reader.ReadToEnd(), settings);
                CourseValidator.ValidateCourse(course, SupportedLanguages.Keys.ToHashSet());
                foreach (var card in course!.Cards)
                {
                    card.SanitizeCourseCard(); 
                }
                result.Add(course);
            }

            if (!result.Any())
            {
                throw new Exception("No courses were loaded");
            }
            return result.OrderBy(x => x.Weight).ThenBy(x => x.Id).ToArray();
        }

        private static readonly Dictionary<string, int> SupportedLanguages = new() {{"en", 0}, {"ru", 1}, {"es", 2}};

        private static string[] SortLanguages(IEnumerable<string> languages)
        {
            var endWeight = int.MaxValue;
            return languages.Select(x =>
            {
                if (!SupportedLanguages.TryGetValue(x, out var weight))
                {
                    weight = endWeight--;
                }

                return new {language = x, weight};
            }).OrderBy(x => x.weight).ThenBy(x => x.language).Select(x => x.language).ToArray();
        }
    }
}