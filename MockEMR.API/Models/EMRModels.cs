namespace MockEMR.API.Models
{
    public class EmrPatient
    {
        public string PatientId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EmrProcedure
    {
        public string ProcedureId { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string ProcedureType { get; set; } = string.Empty;
        public string ProcedureCode { get; set; } = string.Empty;
        public DateTime ProcedureDate { get; set; }
        public decimal Cost { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePatientRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class CreateProcedureRequest
    {
        public string PatientId { get; set; } = string.Empty;
        public string ProcedureType { get; set; } = string.Empty;
        public string ProcedureCode { get; set; } = string.Empty;
        public DateTime ProcedureDate { get; set; }
        public decimal Cost { get; set; }
        public string Status { get; set; } = "Completed";
    }
}
