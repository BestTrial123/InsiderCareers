namespace InsiderCareers.Models
{
    public class Employer
    {
        public int Id { get; set; }
        public required string CompanyName { get; set; }
        public required string ContactName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string StreetAddress { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string Zip { get; set; }
        public required string BusinessType { get; set; }
        public int? EmployeeCount { get; set; }
        public required string Password { get; set; }
        public string? WebsiteUrl { get; set; }
public string? OperatingRegion { get; set; }   // remote / local operation region(s)
public string? DirectPhone { get; set; }        // personal contact number
public string? TaxId { get; set; }              // EIN or VAT number
public string? ContactPosition { get; set; }    // person's job title
public string? VerificationDocumentPath { get; set; } // uploaded license/certificate
public string? LogoPath { get; set; }
public bool InterviewStatus { get; set;} = false;
    }
}
