using System.ComponentModel.DataAnnotations;

namespace ChatApp.MiddleLayer.DTOs
{
    public class UserDTO
    {
        public string Email { get; set; }
        public string? Name { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
