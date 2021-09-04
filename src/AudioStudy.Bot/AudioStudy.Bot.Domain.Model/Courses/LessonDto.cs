using System.Collections.Generic;

namespace AudioStudy.Bot.Domain.Model.Courses
{
    public class LessonDto
    {
        public Dictionary<string, string> FileIds { get; set; }
        public Card[] Cards { get; set; }
    }
}