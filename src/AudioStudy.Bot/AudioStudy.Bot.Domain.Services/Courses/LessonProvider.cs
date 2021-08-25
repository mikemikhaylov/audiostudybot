using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AudioStudy.Bot.Courses;
using AudioStudy.Bot.Domain.Model.Courses;

namespace AudioStudy.Bot.Domain.Services.Courses
{
    public class LessonProvider : ILessonProvider
    {
        private static readonly Lazy<CourseLessons[]> CourseLessonsLazy = new(GetCourseLessons);

        private static readonly Lazy<Dictionary<string, Dictionary<int, Lesson[]>>> LessonsByCourse = new(() =>
        {
            return CourseLessonsLazy.Value
                .GroupBy(x => x.CourseId)
                .ToDictionary(x => x.Key,
                    x => x.ToDictionary(xx => xx.CourseVersion, xx => xx.Lessons));
        });

        public Lesson[] GetCourseLessons(string courseId, int courseVersion)
        {
            if (LessonsByCourse.Value.TryGetValue(courseId, out var byVersion))
            {
                if (byVersion.TryGetValue(courseVersion, out var lessons))
                {
                    if (lessons != null)
                    {
                        return lessons;
                    }
                }
            }

            return Array.Empty<Lesson>();
        }

        public bool TryGetNextLesson(string courseId, int courseVersion, int currentLesson, out Lesson lesson,
            out int lessonNumber)
        {
            lessonNumber = currentLesson < 0 ? 0 : currentLesson + 1;
            var lessons = GetCourseLessons(courseId, courseVersion);
            if (lessonNumber < lessons.Length)
            {
                lesson = lessons[lessonNumber];
                return true;
            }

            lessonNumber = 0;
            lesson = null;
            return false;
        }

        public void Load()
        {
            var tmp = CourseLessonsLazy.Value;
            var tmp2 = LessonsByCourse.Value;
        }

        private static CourseLessons[] GetCourseLessons()
        {
            var result = new List<CourseLessons>();
            var assembly = typeof(CoursesAnchor).Assembly;
            var resources = assembly.GetManifestResourceNames()
                .Where(x => x.StartsWith($"{assembly.GetName().Name}.lessons.") && x.EndsWith(".json"));
            foreach (var resource in resources)
            {
                using var stream = typeof(CoursesAnchor).Assembly.GetManifestResourceStream(resource);
                using var reader = new StreamReader(stream!);
                var courseLessons = JsonSerializer.Deserialize<CourseLessons>(reader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                CourseValidator.ValidateCourseLessons(courseLessons);
                result.Add(courseLessons);
            }

            return result.ToArray();
        }
    }
}