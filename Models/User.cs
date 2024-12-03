using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class User
    {
        public int Id { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 8 and 15 characters and contain at least one lowercase letter, one uppercase letter, one numeric digit, and one special character.")]
        public string PasswordHash { get; set; }

        public string? NewPassword { get; set; }

        public string Email { get; set; }

        public List<string > Roles { get; set; }
    }
}
