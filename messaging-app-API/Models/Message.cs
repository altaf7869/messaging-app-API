using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace messaging_app_API.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }
        public int userId { get; set; }
        public string? User { get; set; }
        public string message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
