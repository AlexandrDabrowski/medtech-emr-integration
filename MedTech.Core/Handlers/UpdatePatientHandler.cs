using MedTech.Core.Commands;
using MedTech.Core.Interfaces;

namespace MedTech.Core.Handlers
{
    public class UpdatePatientHandler
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IIntegrationLogRepository _logRepository;

        public UpdatePatientHandler(IPatientRepository patientRepository, IIntegrationLogRepository logRepository)
        {
            _patientRepository = patientRepository;
            _logRepository = logRepository;
        }

        public async Task<UpdatePatientResult> HandleAsync(UpdatePatientCommand command)
        {
            try
            {
                var patient = await _patientRepository.GetByEmrIdAsync(command.EmrPatientId);
                if (patient == null)
                {
                    await _logRepository.LogAsync(
                        "PatientUpdate",
                        "Error",
                        System.Text.Json.JsonSerializer.Serialize(command),
                        "EMR",
                        "Patient not found"
                    );

                    return new UpdatePatientResult(false, "Patient not found");
                }

                // Update only provided fields
                if (!string.IsNullOrEmpty(command.FirstName))
                    patient.FirstName = command.FirstName;

                if (!string.IsNullOrEmpty(command.LastName))
                    patient.LastName = command.LastName;

                if (!string.IsNullOrEmpty(command.Email))
                    patient.Email = command.Email;

                if (!string.IsNullOrEmpty(command.Phone))
                    patient.Phone = command.Phone;

                if (command.DateOfBirth.HasValue)
                    patient.DateOfBirth = command.DateOfBirth.Value;

                await _patientRepository.UpdateAsync(patient);

                await _logRepository.LogAsync(
                    "PatientUpdate",
                    "Success",
                    System.Text.Json.JsonSerializer.Serialize(command),
                    "EMR"
                );

                return new UpdatePatientResult(true);
            }
            catch (Exception ex)
            {
                await _logRepository.LogAsync(
                    "PatientUpdate",
                    "Error",
                    System.Text.Json.JsonSerializer.Serialize(command),
                    "EMR",
                    ex.Message
                );

                return new UpdatePatientResult(false, ex.Message);
            }
        }
    }
}
