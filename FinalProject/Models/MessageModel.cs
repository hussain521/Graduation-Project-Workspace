namespace FinalProject.Models
{
    public class MessageType
    {
        public const int Success = 1;
        public const int Information = 2;
        public const int Warning = 3;
        public const int Error = 4;
    }

    public class MessageModel
    {
        public string Text { get; set; }
        public int Duration { get; set; } = 10;
        public int Type { get; set; } = MessageType.Success;
    }
}
