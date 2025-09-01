namespace MedTech.Core.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string EmrPatientId { get; set; } = string.Empty;
        public string MedTechPatientId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public SyncStatus SyncStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<Procedure> Procedures { get; set; } = new();
    }

    public enum SyncStatus
    {
        Pending,
        Synced,
        Failed,
        Conflict
    }
}
