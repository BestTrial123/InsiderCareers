namespace InsiderCareers.Models
{
    public class Job
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public string? Location { get; set; }
        public string? JobType { get; set; } // Full-time, Part-time, Contract
        public string? Salary { get; set; }    // e.g. "$15 per hour"
        public string Status { get; set; } = "Open"; // Open, Paused, Closed
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;
        public int ClickCount { get; set; } = 0;
        public int EmployerId { get; set; }
        public Employer? Employer { get; set; }

        public List<JobApplication> Applications { get; set; } = new();
    }
}
