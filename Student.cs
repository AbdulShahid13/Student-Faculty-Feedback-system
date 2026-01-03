using System.ComponentModel.DataAnnotations;

namespace FeedbackSystem.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{3}\d{2}[A-Z]{2}\d{3}$")]
        [Display(Name = "Register Number")]
        public required string RegisterNumber { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public required string FullName { get; set; }

        [Required]
        public required string Department { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        public int Semester { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Feedback>? Feedbacks { get; set; }
    }
}