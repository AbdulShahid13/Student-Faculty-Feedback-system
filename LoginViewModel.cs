using System.ComponentModel.DataAnnotations;

namespace FeedbackSystem.Models
{
    public class AdminLoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public required string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; }
    }

    public class TeacherLoginViewModel
    {
        [Required]
        [Display(Name = "Register Number")]
        public required string TeacherId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; }
    }

    public class StudentLoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; }
    }
}
