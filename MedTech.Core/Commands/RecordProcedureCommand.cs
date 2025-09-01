using MedTech.Core.Models;

namespace MedTech.Core.Commands
{
    public record RecordProcedureCommand(
        string EmrPatientId,
        string EmrReferenceId,
        string ProcedureType,
        string ProcedureCode,
        DateTime ProcedureDate,
        decimal Cost,
        ProcedureStatus Status = ProcedureStatus.Completed
    );

    public record RecordProcedureResult(
        bool Success,
        int? ProcedureId = null,
        int PointsEarned = 0,
        string? ErrorMessage = null
    );
}
