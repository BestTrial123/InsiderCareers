using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

namespace InsiderCareers.Models
{
    public class AppDbContext : DbContext, IDataProtectionKeyContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employer> Employers => Set<Employer>();
        public DbSet<JobSeeker> JobSeekers => Set<JobSeeker>();
        public DbSet<Admin> Admins { get; set; }

        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<Message> Messages { get; set; }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}

