using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace messaging_app_API.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Token { get; set; }
        public string? ResetPasswordToken { get; set; } // Add this property
        public DateTime ResetPasswordExpiry { get; set; } // Add this property

    }
}
