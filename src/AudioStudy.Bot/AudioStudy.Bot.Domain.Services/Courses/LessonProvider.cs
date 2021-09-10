using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AudioStudy.Bot.Courses;
using AudioStudy.Bot.Domain.Model.Courses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AudioStudy.Bot.Domain.Services.Courses
{
    public class LessonProvider : ILessonProvider
    {
        private readonly IOptions<LessonProviderOptions> _config;
        private readonly ILogger<LessonProvider> _logger;

        private static bool _lessonsByCourseSet;
        private static readonly object LessonsByCourseLock = new();
        private static Dictionary<string, Dictionary<int, Lesson[]>> _lessonsByCourse;
        
        public LessonProvider(IOptions<LessonProviderOptions> config, ILogger<LessonProvider> logger)
        {
            _config = config;
            _logger = logger;
        }

        public Lesson[] GetCourseLessons(string courseId, int courseVersion)
        {
            if (!_lessonsByCourseSet)
            {
                lock (LessonsByCourseLock)
                {
                    if (!_lessonsByCourseSet)
                    {
                        _lessonsByCourse = GetCourseLessons()
                        .GroupBy(x => x.CourseId)
                        .ToDictionary(x => x.Key,
                            x => x.ToDictionary(xx => xx.CourseVersion, xx => xx.Lessons));
                        _lessonsByCourseSet = true;
                    }
                }
            }
            if (_lessonsByCourse.TryGetValue(courseId, out var byVersion))
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
            _logger.LogInformation("Starting loading lessons at: {time}.", DateTimeOffset.UtcNow);
            var tmp = GetCourseLessons(string.Empty, 0);
            _logger.LogInformation("Finished loading lessons at: {time}.", DateTimeOffset.UtcNow);
        }

        private CourseLessons[] GetCourseLessons()
        {
            var result = new List<CourseLessons>();
            var assembly = typeof(CoursesAnchor).Assembly;
            var resources = assembly.GetManifestResourceNames()
                .Where(x => x.StartsWith($"{assembly.GetName().Name}.lessons.") && x.EndsWith(".json"));
            foreach (var resource in resources)
            {
                using var stream = typeof(CoursesAnchor).Assembly.GetManifestResourceStream(resource);
                using var reader = new StreamReader(stream!);
                var courseLessonsDto = JsonSerializer.Deserialize<CourseLessonsDto>(reader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                var courseLessons = new CourseLessons
                {
                    CourseId = courseLessonsDto!.CourseId,
                    CourseVersion = courseLessonsDto.CourseVersion,
                    Lessons = courseLessonsDto.Lessons?.Select(x =>
                    {
                        string fileId = null;
                        if (x.FileIds != null)
                        {
                            x.FileIds.TryGetValue(_config.Value.BotName, out fileId);
                        }

                        return new Lesson
                        {
                            FileId = fileId,
                            Cards = x.Cards
                        };
                    }).ToArray()
                };
                CourseValidator.ValidateCourseLessons(courseLessons);
                result.Add(courseLessons);
            }
            if (!result.Any())
            {
                throw new Exception("No lessons were loaded");
            }
            return result.ToArray();
        }
    }
}