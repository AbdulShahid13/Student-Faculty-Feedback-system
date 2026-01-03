namespace FeedbackSystem.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public required string TeacherId { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public required string Department { get; set; }
        public required string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TeacherSubject> TeacherSubjects { get; set; } = new();
    }
}