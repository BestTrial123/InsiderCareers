using Microsoft.EntityFrameworkCore;

namespace InsiderCareers.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employer> Employers => Set<Employer>();
        public DbSet<JobSeeker> JobSeekers => Set<JobSeeker>();
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<Message> Messages { get; set; }
    }
}
