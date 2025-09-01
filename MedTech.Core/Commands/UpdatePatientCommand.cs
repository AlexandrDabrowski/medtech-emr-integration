namespace MedTech.Core.Commands
{
    public record UpdatePatientCommand(
      string EmrPatientId,
      string? FirstName = null,
      string? LastName = null,
      string? Email = null,
      string? Phone = null,
      DateTime? DateOfBirth = null
  );

    public record UpdatePatientResult(
        bool Success,
        string? ErrorMessage = null
    );
}
