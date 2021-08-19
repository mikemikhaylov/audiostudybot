namespace AudioStudy.Bot.DataAccess.Abstractions
{
    public class FieldUpdateCommand<T>
    {
        public T Value { get; set; }

        public FieldUpdateCommand(T value)
        {
            Value = value;
        }
    }
}