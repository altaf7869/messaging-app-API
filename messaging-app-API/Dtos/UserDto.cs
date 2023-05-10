﻿using System.ComponentModel.DataAnnotations;

namespace messaging_app_API.Dtos
{
    public class UserDto
    {
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Name must be at least 2, and maximum 1 character")]
        public string Name { get; set; }
    }
}
