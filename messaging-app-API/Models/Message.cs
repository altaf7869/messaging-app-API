namespace messaging_app_API.Models
{
    public class Message
    {
        public string userId { get; set; }
        public string type { get; set; }
        public string message { get; set; }
        public DateTime date { get; set; }
    }
}
