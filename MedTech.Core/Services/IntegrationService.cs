using MedTech.Core.Commands;
using MedTech.Core.Handlers;
using MedTech.Core.Interfaces;

namespace MedTech.Core.Services
{
    public interface IIntegrationService
    {
        Task<CreatePatientResult> SyncPatientAsync(CreatePatientCommand command);
        Task<RecordProcedureResult> ProcessProcedureAsync(RecordProcedureCommand command);
        Task<bool> RetryFailedSyncsAsync();
    }

    public class IntegrationService : IIntegrationService
    {
        private readonly CreatePatientHandler _createPatientHandler;
        private readonly RecordProcedureHandler _recordProcedureHandler;
        private readonly IIntegrationLogRepository _logRepository;

        public IntegrationService(
            CreatePatientHandler createPatientHandler,
            RecordProcedureHandler recordProcedureHandler,
            IIntegrationLogRepository logRepository)
        {
            _createPatientHandler = createPatientHandler;
            _recordProcedureHandler = recordProcedureHandler;
            _logRepository = logRepository;
        }

        public async Task<CreatePatientResult> SyncPatientAsync(CreatePatientCommand command)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = await _createPatientHandler.HandleAsync(command);
            stopwatch.Stop();

            await _logRepository.LogAsync(
                "PatientSync",
                result.Success ? "Success" : "Failed",
                System.Text.Json.JsonSerializer.Serialize(command),
                "IntegrationService",
                result.ErrorMessage,
                $"{stopwatch.ElapsedMilliseconds}ms"
            );

            return result;
        }

        public async Task<RecordProcedureResult> ProcessProcedureAsync(RecordProcedureCommand command)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = await _recordProcedureHandler.HandleAsync(command);
            stopwatch.Stop();

            await _logRepository.LogAsync(
                "ProcedureProcess",
                result.Success ? "Success" : "Failed",
                System.Text.Json.JsonSerializer.Serialize(command),
                "IntegrationService",
                result.ErrorMessage,
                $"{stopwatch.ElapsedMilliseconds}ms"
            );

            return result;
        }

        public async Task<bool> RetryFailedSyncsAsync()
        {
            // MVP: Simple retry logic for failed syncs in last 24 hours
            var failedLogs = await _logRepository.GetErrorLogsAsync(DateTime.UtcNow.AddDays(-1));

            foreach (var log in failedLogs.Take(10)) // Limit retries for MVP
            {
                try
                {
                    // In a full implementation, we'd deserialize and retry the original command
                    // For MVP, just log the retry attempt
                    await _logRepository.LogAsync(
                        $"{log.EventType}Retry",
                        "Attempted",
                        log.Payload,
                        "RetryService",
                        "Manual retry initiated"
                    );
                }
                catch (Exception ex)
                {
                    await _logRepository.LogAsync(
                        $"{log.EventType}Retry",
                        "Failed",
                        log.Payload,
                        "RetryService",
                        ex.Message
                    );
                }
            }

            return true;
        }
    }
}
