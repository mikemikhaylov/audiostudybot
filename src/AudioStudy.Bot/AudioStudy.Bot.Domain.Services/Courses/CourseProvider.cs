using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AudioStudy.Bot.Courses;
using AudioStudy.Bot.Domain.Model.Courses;

namespace AudioStudy.Bot.Domain.Services.Courses
{
    public class CourseProvider : ICourseProvider
    {
        private static readonly Lazy<Course[]> Courses = new(GetAllCourses);

        private static readonly Lazy<string[]> CoursesLanguages = new(() => Courses.Value.SelectMany(x =>
        {
            return x.CanBeReversed ? new[] {x.Language, x.TranslationLanguage} : new[] {x.Language};
        }).ToArray());

        private static readonly Lazy<Dictionary<string, string[]>> TranslationLanguages = new(() =>
        {
            var result = new Dictionary<string, HashSet<string>>();
            foreach (var languagePairs in Courses.Value.SelectMany(x =>
            {
                if (x.CanBeReversed)
                {
                    return new[] {new[] {x.Language, x.TranslationLanguage}, new[] {x.TranslationLanguage, x.Language}};
                }
                return new[] {new[] {x.Language, x.TranslationLanguage}};
            }))
            {
                if (!result.ContainsKey(languagePairs[0]))
                {
                    result[languagePairs[0]] = new HashSet<string>();
                }
                result[languagePairs[0]].Add(languagePairs[1]);
            }
            return result.ToDictionary(x => x.Key, x => x.Value.ToArray());
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

        private static Course[] GetAllCourses()
        {
            var result = new List<Course>();
            var resources = typeof(CoursesAnchor).Assembly.GetManifestResourceNames().Where(x => x.EndsWith(".json"));
            foreach (var resource in resources)
            {
                using var stream = typeof(CoursesAnchor).Assembly.GetManifestResourceStream(resource);
                using var reader = new StreamReader(stream!);
                var course = JsonSerializer.Deserialize<Course>(reader.ReadToEnd());
                ValidateCourse(course);
                result.Add(course);
            }

            return result.ToArray();
        }

        private static readonly HashSet<string> SupportedLanguages = new HashSet<string>() {"en", "ru", "es"};

        private static void ValidateCourse(Course course)
        {
            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }

            if (string.IsNullOrWhiteSpace(course.Id))
            {
                throw new Exception($"{nameof(course.Id)} CourseId required");
            }

            if (string.IsNullOrWhiteSpace(course.Name))
            {
                throw new Exception($"{nameof(course.Name)} is required. CourseId {course.Id}");
            }

            if (string.IsNullOrWhiteSpace(course.Description))
            {
                throw new Exception($"{nameof(course.Description)} is required. CourseId {course.Id}");
            }

            if (string.IsNullOrWhiteSpace(course.Language))
            {
                throw new Exception($"{nameof(course.Language)} is required. CourseId {course.Id}");
            }

            if (!SupportedLanguages.Contains(course.Language))
            {
                throw new Exception($"{course.Language} language is not supported. CourseId {course.Id}");
            }

            if (string.IsNullOrWhiteSpace(course.TranslationLanguage))
            {
                throw new Exception($"{nameof(course.TranslationLanguage)} is required. CourseId {course.Id}");
            }

            if (!SupportedLanguages.Contains(course.TranslationLanguage))
            {
                throw new Exception($"{course.TranslationLanguage} language is not supported. CourseId {course.Id}");
            }

            if (course.Cards == null || !course.Cards.Any())
            {
                throw new Exception($"{nameof(course.Cards)} should contain items. CourseId {course.Id}");
            }

            CheckTextLength(course.Id, course.Name, course.Description, course.NameTranslation,
                course.DescriptionTranslation);
            foreach (var card in course.Cards)
            {
                CheckTextLength(card.Text, card.Translation, card.Transcription, card.Usage, card.UsageTranslation);
                if (string.IsNullOrWhiteSpace(card.Text))
                {
                    throw new Exception($"{nameof(card.Text)} is required");
                }

                if (string.IsNullOrWhiteSpace(card.Translation))
                {
                    throw new Exception($"{nameof(card.Translation)} is required. Text: {card.Text}");
                }
            }
        }

        private const int MaxTextLength = 1000;

        private static void CheckTextLength(params string[] texts)
        {
            if (texts.Any(x => x is {Length: > MaxTextLength}))
            {
                throw new Exception("Max text length violation");
            }
        }
    }
}