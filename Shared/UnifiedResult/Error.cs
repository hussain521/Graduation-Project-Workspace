namespace Shared.UnifiedResult
{
    public class Error
    {
        public Error()
        {
            Messages = new List<string>();
        }
        public Error(string message)
        {
            Messages = new List<string> { message };
        }

        public Error(string[] messages)
        {
            Messages = messages.ToList();
        }

        public Error(List<string> messages)
        {
            Messages = messages;
        }

        public List<string> Messages { get; }

        public static Error None => new(string.Empty);

        public static implicit operator Error(string message) => new(message);

        public static implicit operator string(Error error) => error.Messages?.First();
    }
}
