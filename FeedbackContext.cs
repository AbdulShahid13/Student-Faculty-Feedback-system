using Microsoft.EntityFrameworkCore;

namespace FeedbackSystem.Models
{
    public class FeedbackContext : DbContext
    {
        public FeedbackContext(DbContextOptions<FeedbackContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeacherSubject>()
                .HasOne(ts => ts.Teacher)
                .WithMany(t => t.TeacherSubjects)
                .HasForeignKey(ts => ts.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}