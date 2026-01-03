namespace FeedbackSystem.Models
{
    public class FeedbackViewModel
    {
        public int Semester { get; set; }
        public required string Department { get; set; }
        public int Q1 { get; set; }
        public int Q2 { get; set; }
        public int Q3 { get; set; }
        public int Q4 { get; set; }
        public int Q5 { get; set; }
        public int Q6 { get; set; }
        public int Q7 { get; set; }
        public int Q8 { get; set; }
        public int Q9 { get; set; }
        public int Q10 { get; set; }
        public required string Comments { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
