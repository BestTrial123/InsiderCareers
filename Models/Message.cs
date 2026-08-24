namespace InsiderCareers.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int EmployerId { get; set; }
        public required string SenderName { get; set; }
        public required string SenderEmail { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public DateTime SentDate { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
        public string? AttachmentPath { get; set; }
        public string? AttachmentFileName { get; set; }
        public string? RecipientEmail { get; set; }
    }
}