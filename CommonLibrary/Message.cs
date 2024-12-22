namespace CommonLibrary
{
    public class Message
    {
        public string? type { get; set; }
        public string? action { get; set; }
        public Dictionary<string, string>? data { get; set; }
        public Dictionary<string, string>? error { get; set; }

        public Message() { }

        public Message(string _type, string _action) {
            type = _type;
            action = _action;
        }

        public Message(string _type, string _action, Dictionary<string, string>? _data = null, Dictionary<string, string>? _error = null)
        {
            type = _type;
            action = _action;
            data = _data;
            error = _error;
        }
    }
}
