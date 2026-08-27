namespace InsiderCareers.Models
{
    public class Message
    {
        public int Id { get; set; }
        public required string SenderType { get; set; }
        public int SenderId { get; set; }
        public required string SenderName { get; set; }
        public required string RecipientType { get; set; }
        public int RecipientId { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public DateTime SentDate { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
        public string? AttachmentPath { get; set; }
        public string? AttachmentFileName { get; set; }
    }
}
