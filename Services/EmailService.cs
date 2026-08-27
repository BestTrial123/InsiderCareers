using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsiderCareers.Services
{
    public class EmailService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = string.Empty;

        public EmailService(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _apiKey = config["RESEND_API_KEY"] ?? throw new InvalidOperationException("RESEND_API_KEY not configured.");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var payload = new
            {
                from = "Insider Careers <onboarding@resend.dev>",
                to = new[] { toEmail },
                subject = subject,
                html = htmlBody
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
