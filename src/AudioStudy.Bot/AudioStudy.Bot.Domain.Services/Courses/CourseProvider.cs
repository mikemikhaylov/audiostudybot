using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AudioStudy.Bot.Courses;
using AudioStudy.Bot.Domain.Model.Courses;
using AudioStudy.Bot.SharedUtils.Localization.Enums;

namespace AudioStudy.Bot.Domain.Services.Courses
{
    public class CourseProvider : ICourseProvider
    {
        private static readonly Lazy<Course[]> Courses = new(GetAllCourses);

        private static readonly Lazy<string[]> CoursesLanguages = new(() => SortLanguages(Courses.Value.SelectMany(x =>
        {
            return x.CanBeReversed ? new[] {x.Language, x.TranslationLanguage} : new[] {x.Language};
        }).Distinct()));

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
            return Courses.Value.Where(x => x.Language == language && x.TranslationLanguage == translationLanguage
                || (x.CanBeReversed && x.Language == translationLanguage && x.TranslationLanguage == language))
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

        public void Load()
        {
            var loaded = Courses.Value;
        }

        private static Course[] GetAllCourses()
        {
            var result = new List<Course>();
            var resources = typeof(CoursesAnchor).Assembly.GetManifestResourceNames().Where(x => x.EndsWith(".json"));
            foreach (var resource in resources)
            {
                using var stream = typeof(CoursesAnchor).Assembly.GetManifestResourceStream(resource);
                using var reader = new StreamReader(stream!);
                var course = JsonSerializer.Deserialize<Course>(reader.ReadToEnd(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                ValidateCourse(course);
                result.Add(course);
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

            if (!SupportedLanguages.ContainsKey(course.Language))
            {
                throw new Exception($"{course.Language} language is not supported. CourseId {course.Id}");
            }

            if (string.IsNullOrWhiteSpace(course.TranslationLanguage))
            {
                throw new Exception($"{nameof(course.TranslationLanguage)} is required. CourseId {course.Id}");
            }

            if (!SupportedLanguages.ContainsKey(course.TranslationLanguage))
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