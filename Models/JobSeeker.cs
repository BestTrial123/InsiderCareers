namespace InsiderCareers.Models
{
    public class JobSeeker
    {
        public int Id { get; set; }

        // Basic Info
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? MaritalStatus { get; set; }
        public string? LanguageSpoken { get; set; }

        // Photo
        public string? PhotoPath { get; set; }

        // Documents
        public string? ResumePath { get; set; }
        public string? CoverLetter { get; set; }

        // Profile details
        public string? Skills { get; set; }        // comma-separated for now
        public string? WorkHistory { get; set; }    // comma-separated for now
        public decimal? SalaryDesire { get; set; }

        // Location
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }

        public List<JobApplication> Applications { get; set; } = new();
    }
}
