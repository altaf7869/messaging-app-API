using System.ComponentModel.DataAnnotations;

namespace messaging_app_API.Dtos
{
    public class ChatDto
    {
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Name must be at least {2}, and maximum {1} characters")]
        public string Name { get; set; }

    }
}