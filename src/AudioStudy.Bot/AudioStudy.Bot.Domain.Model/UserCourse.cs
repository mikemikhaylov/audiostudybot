namespace AudioStudy.Bot.Domain.Model
{
    public class UserCourse
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public int LastLesson { get; set; } = -1;
    }
}