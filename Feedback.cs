using System.ComponentModel.DataAnnotations;

namespace FeedbackSystem.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public int Semester { get; set; }

        public required string Department { get; set; }

        public DateTime CreatedAt { get; set; }

        public required string KnowledgeInSubject { get; set; }

        public required string AbilityToExpressIdeas { get; set; }

        public required string AbilityToGiveExamples { get; set; }

        public required string AbilityToArouseInterest { get; set; }

        public required string AbilityToAttractAttention { get; set; }

        public required string GeneralBehavior { get; set; }

        public required string WillingnessToHelp { get; set; }

        public required string CommandOfEnglish { get; set; }

        public required string Punctuality { get; set; }

        public required string PreparationAndSincerity { get; set; }

        public string? Comments { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Student? Student { get; set; }
        public virtual Teacher? Teacher { get; set; }
    }
}