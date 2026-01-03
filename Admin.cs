using System;
using System.ComponentModel.DataAnnotations;

namespace FeedbackSystem.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Username { get; set; }

        [Required]
        [StringLength(100)]
        public required string Password { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
