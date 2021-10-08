using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using AudioStudy.Bot.Domain.Model.Courses;
using AudioStudy.Bot.Domain.Services.Telegram;

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

            if (course.Type != CourseType.Words && course.Type != CourseType.Phrases)
            {
                throw new Exception($"{course.Type} course type is not supported. CourseId {course.Id}");
            }
            
            CheckTextLength(course.Id, course.Name, course.Description, course.NameTranslation,
                course.DescriptionTranslation);

            ValidateCards(course.Id, course.Cards);
        }

        private static readonly JsonSerializerOptions Options = new()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private static void ValidateCards(string courseId, IList<Card> cards)
        {
            if (cards == null || !cards.Any())
            {
                throw new Exception($"{nameof(cards)} should contain items. CourseId {courseId}");
            }

            foreach (var batch in cards.Select((item, inx) => new { item, inx })
                .GroupBy(x => x.inx / Consts.CardsPerPage)
                .Select(g => g.Select(x => x.item)))
            {
                CheckTextLength(string.Join(Environment.NewLine, batch.Select(x=> 
                    $"{x.Text} {x.Transcription} {x.Translation} {x.Usage} {x.UsageTranslation}")));
            }
            foreach (var card in cards)
            {
                ValidateCard(card);
            }
        }
        
        private static void ValidateCard(Card card)
        {
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
                ValidateCards(courseLessons.CourseId, lesson.Cards);
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