using System.ComponentModel.DataAnnotations;

namespace messaging_app_API.Dto
{
    public class UserloginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
        public string Password { get; set; }
        
    }
}
