using System;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationAPI.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string DisplayName { get; set; }
    }
}
