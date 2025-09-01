using MedTech.Core.Commands;
using MedTech.Core.Interfaces;
using MedTech.Core.Models;
using MedTech.Core.Services;

namespace MedTech.Core.Handlers
{
    public class RecordProcedureHandler
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IProcedureRepository _procedureRepository;
        private readonly IIntegrationLogRepository _logRepository;
        private readonly IPointsCalculator _pointsCalculator;

        public RecordProcedureHandler(
            IPatientRepository patientRepository,
            IProcedureRepository procedureRepository,
            IIntegrationLogRepository logRepository,
            IPointsCalculator pointsCalculator)
        {
            _patientRepository = patientRepository;
            _procedureRepository = procedureRepository;
            _logRepository = logRepository;
            _pointsCalculator = pointsCalculator;
        }

        public async Task<RecordProcedureResult> HandleAsync(RecordProcedureCommand command)
        {
            try
            {
                // Check if procedure already exists
                if (await _procedureRepository.ExistsAsync(command.EmrReferenceId))
                {
                    await _logRepository.LogAsync(
                        "ProcedureRecord",
                        "Skipped",
                        System.Text.Json.JsonSerializer.Serialize(command),
                        "EMR",
                        "Procedure already exists"
                    );

                    return new RecordProcedureResult(true, null, 0, "Procedure already exists");
                }

                var patient = await _patientRepository.GetByEmrIdAsync(command.EmrPatientId);
                if (patient == null)
                {
                    await _logRepository.LogAsync(
                        "ProcedureRecord",
                        "Error",
                        System.Text.Json.JsonSerializer.Serialize(command),
                        "EMR",
                        "Patient not found"
                    );

                    return new RecordProcedureResult(false, null, 0, "Patient not found");
                }

                var pointsEarned = command.Status == ProcedureStatus.Completed
                    ? _pointsCalculator.CalculatePoints(command.ProcedureCode, command.Cost)
                    : 0;

                var procedure = new Procedure
                {
                    PatientId = patient.Id,
                    EmrReferenceId = command.EmrReferenceId,
                    ProcedureType = command.ProcedureType,
                    ProcedureCode = command.ProcedureCode,
                    ProcedureDate = command.ProcedureDate,
                    Cost = command.Cost,
                    PointsEarned = pointsEarned,
                    Status = command.Status
                };

                var created = await _procedureRepository.CreateAsync(procedure);

                await _logRepository.LogAsync(
                    "ProcedureRecord",
                    "Success",
                    System.Text.Json.JsonSerializer.Serialize(command),
                    "EMR"
                );

                return new RecordProcedureResult(true, created.Id, pointsEarned);
            }
            catch (Exception ex)
            {
                await _logRepository.LogAsync(
                    "ProcedureRecord",
                    "Error",
                    System.Text.Json.JsonSerializer.Serialize(command),
                    "EMR",
                    ex.Message
                );

                return new RecordProcedureResult(false, null, 0, ex.Message);
            }
        }
    }
}
