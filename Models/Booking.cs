namespace InsiderCareers.Models
{
    public class Booking
    {
        public string Name { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Time { get; set; } = "";
        public string Room { get; set; } = "";
        public bool HasSms { get; set; }
        public string Status { get; set; } = "Pending"; // Confirmed, Pending, Canceled
    }
}
