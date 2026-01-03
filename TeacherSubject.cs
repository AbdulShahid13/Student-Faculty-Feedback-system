namespace FeedbackSystem.Models
{
    public class TeacherSubject
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public required string Subject { get; set; }
        public required int Semester { get; set; }
        public Teacher Teacher { get; set; } = null!;
    }
}
