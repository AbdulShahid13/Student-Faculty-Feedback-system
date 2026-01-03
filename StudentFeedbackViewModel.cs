using System;

namespace FeedbackSystem.Models
{
    public class StudentFeedbackViewModel
    {
        public required string TeacherName { get; set; }
        public required string Department { get; set; }
        public required string KnowledgeRating { get; set; }
        public required string ExpressIdeasRating { get; set; }
        public required string ExamplesRating { get; set; }
        public required string InterestRating { get; set; }
        public required string AttentionRating { get; set; }
        public required string BehaviorRating { get; set; }
        public required string HelpRating { get; set; }
        public required string EnglishRating { get; set; }
        public required string PunctualityRating { get; set; }
        public required string PreparationRating { get; set; }
        public int Semester { get; set; }
        public string? Comments { get; set; }
        public required string SubmittedDate { get; set; }
    }
}
