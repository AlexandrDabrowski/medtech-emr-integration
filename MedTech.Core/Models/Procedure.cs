namespace MedTech.Core.Models
{
    public class Procedure
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string EmrReferenceId { get; set; } = string.Empty;
        public string ProcedureType { get; set; } = string.Empty;
        public string ProcedureCode { get; set; } = string.Empty;
        public DateTime ProcedureDate { get; set; }
        public decimal Cost { get; set; }
        public int PointsEarned { get; set; }
        public ProcedureStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public Patient Patient { get; set; } = null!;
    }

    public enum ProcedureStatus
    {
        Completed,
        Cancelled,
        NoShow,
        Refunded
    }
}
