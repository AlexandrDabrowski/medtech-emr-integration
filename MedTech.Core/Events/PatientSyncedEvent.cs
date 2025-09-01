namespace MedTech.Core.Events
{
    public record PatientSyncedEvent(
       string EmrPatientId,
       string MedTechPatientId,
       DateTime SyncedAt
   );

    public record ProcedureCompletedEvent(
        string EmrPatientId,
        string ProcedureType,
        int PointsEarned,
        DateTime CompletedAt
    );

    public record SyncFailedEvent(
        string EntityType,
        string EntityId,
        string ErrorMessage,
        DateTime FailedAt,
        int RetryCount = 0
    );
}
