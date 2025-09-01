namespace MedTech.Core.Models
{
    public class IntegrationLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public string Payload { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string? ProcessingDuration { get; set; }
    }
}
