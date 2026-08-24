namespace InsiderCareers.Models
{
    public class JobSeekerDashboardViewModel
    {
        public required JobSeeker JobSeeker { get; set; }
        public int AppliedCount { get; set; }
        public int SavedCount { get; set; }
        public int ViewedCount { get; set; }
        public int DeniedCount { get; set; }
        public List<JobApplication> InterviewAppointments { get; set; } = new();
        public List<Job> JobFeed { get; set; } = new();
    }
}