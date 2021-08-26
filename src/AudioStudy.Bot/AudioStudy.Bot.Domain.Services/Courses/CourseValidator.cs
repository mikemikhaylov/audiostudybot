using System;
using System.Collections.Generic;
using System.Linq;
using AudioStudy.Bot.Domain.Model.Courses;

namespace AudioStudy.Bot.Domain.Services.Courses
{
    internal static class CourseValidator
    {
        public static void ValidateCourse(Course course, HashSet<string> supportedLanguages)
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

            if (!supportedLanguages.Contains(course.Language))
            {
                throw new Exception($"{course.Language} language is not supported. CourseId {course.Id}");
            }

            if (string.IsNullOrWhiteSpace(course.TranslationLanguage))
            {
                throw new Exception($"{nameof(course.TranslationLanguage)} is required. CourseId {course.Id}");
            }

            if (!supportedLanguages.Contains(course.TranslationLanguage))
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
                ValidateCard(card);
            }
        }

        public static void ValidateCard(Card card)
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
        
        public static void ValidateCourseLessons(CourseLessons courseLessons)
        {
            CheckTextLength(courseLessons.CourseId);
            if (string.IsNullOrWhiteSpace(courseLessons.CourseId))
            {
                throw new Exception($"{nameof(courseLessons.CourseId)} is required");
            }

            if (courseLessons.Lessons == null || !courseLessons.Lessons.Any())
            {
                throw new Exception($"{nameof(courseLessons.Lessons)} is required. CourseId: {courseLessons.CourseId}");
            }

            foreach (var lesson in courseLessons.Lessons)
            {
                if (string.IsNullOrWhiteSpace(lesson.FileId))
                {
                    throw new Exception($"{nameof(lesson.FileId)} is required");
                }
                if (lesson.Cards == null || !lesson.Cards.Any())
                {
                    throw new Exception($"{nameof(lesson.Cards)} is required.");
                }
                foreach (var card in lesson.Cards)
                {
                    ValidateCard(card);
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