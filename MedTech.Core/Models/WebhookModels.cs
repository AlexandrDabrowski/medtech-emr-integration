namespace MedTech.Core.Models
{
    public class EmrPatientWebhook
    {
        public string PatientId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Source { get; set; }
        public DateTime EventTimestamp { get; set; } = DateTime.UtcNow;
    }

    public class EmrProcedureWebhook
    {
        public string PatientId { get; set; } = string.Empty;
        public string ProcedureId { get; set; } = string.Empty;
        public string ProcedureType { get; set; } = string.Empty;
        public string ProcedureCode { get; set; } = string.Empty;
        public DateTime ProcedureDate { get; set; }
        public decimal Cost { get; set; }
        public string Status { get; set; } = "Completed";
        public DateTime EventTimestamp { get; set; } = DateTime.UtcNow;
    }
}
