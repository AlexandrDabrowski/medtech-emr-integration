namespace MedTech.Core.Commands
{
    public record CreatePatientCommand(
        string EmrPatientId,
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        DateTime DateOfBirth,
        string Source = "EMR"
    );

    public record CreatePatientResult(
        bool Success,
        string? PatientId = null,
        string? ErrorMessage = null
    );
}
