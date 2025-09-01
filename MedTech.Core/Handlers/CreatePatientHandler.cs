using MedTech.Core.Commands;
using MedTech.Core.Interfaces;
using MedTech.Core.Models;

namespace MedTech.Core.Handlers
{
    public class CreatePatientHandler
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IIntegrationLogRepository _logRepository;

        public CreatePatientHandler(IPatientRepository patientRepository, IIntegrationLogRepository logRepository)
        {
            _patientRepository = patientRepository;
            _logRepository = logRepository;
        }

        public async Task<CreatePatientResult> HandleAsync(CreatePatientCommand command)
        {
            try
            {
                // Check if patient already exists
                if (await _patientRepository.ExistsAsync(command.EmrPatientId))
                {
                    await _logRepository.LogAsync(
                        "PatientCreate",
                        "Skipped",
                        System.Text.Json.JsonSerializer.Serialize(command),
                        command.Source,
                        "Patient already exists"
                    );

                    var existing = await _patientRepository.GetByEmrIdAsync(command.EmrPatientId);
                    return new CreatePatientResult(true, existing?.MedTechPatientId, "Patient already exists");
                }

                var patient = new Patient
                {
                    EmrPatientId = command.EmrPatientId,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    Email = command.Email,
                    Phone = command.Phone,
                    DateOfBirth = command.DateOfBirth,
                    SyncStatus = SyncStatus.Pending
                };

                var created = await _patientRepository.CreateAsync(patient);
                created.SyncStatus = SyncStatus.Synced;
                await _patientRepository.UpdateAsync(created);

                await _logRepository.LogAsync(
                    "PatientCreate",
                    "Success",
                    System.Text.Json.JsonSerializer.Serialize(command),
                    command.Source
                );

                return new CreatePatientResult(true, created.MedTechPatientId);
            }
            catch (Exception ex)
            {
                await _logRepository.LogAsync(
                    "PatientCreate",
                    "Error",
                    System.Text.Json.JsonSerializer.Serialize(command),
                    command.Source,
                    ex.Message
                );

                return new CreatePatientResult(false, null, ex.Message);
            }
        }
    }
}
