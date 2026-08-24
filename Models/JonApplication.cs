namespace InsiderCareers.Models
{
    public class JobApplication
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public Job? Job { get; set; }
        public int JobSeekerId { get; set; }
        public JobSeeker? JobSeeker { get; set; }
        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;

        // Status values used across the Jobseeker dashboard:
        // "Applied", "Saved", "Viewed", "Denied", "Interview"
        public string Status { get; set; } = "Submitted";

        // Only set when Status == "Interview"
        public DateTime? InterviewDate { get; set; }
    }
}

