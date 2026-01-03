using System.ComponentModel.DataAnnotations;

namespace FeedbackSystem.Models
{
    public class AddTeacherViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Teacher ID is required")]
        [Display(Name = "Teacher ID")]
        public required string TeacherId { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public required string Department { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "At least one subject and semester is required")]
        [Display(Name = "Subjects and Semesters")]
        public List<SubjectSemesterEntry> SubjectSemesters { get; set; } = new();
    }

    public class SubjectSemesterEntry
    {
        public required string Subject { get; set; }
        public required int Semester { get; set; }
    }
}
