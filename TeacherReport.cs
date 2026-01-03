namespace FeedbackSystem.Models
{
    public class TeacherReport
    {
        public required string TeacherName { get; set; }
        public required string Department { get; set; }
        public double AverageRating { get; set; }
        public int FeedbackCount { get; set; }
    }
}
