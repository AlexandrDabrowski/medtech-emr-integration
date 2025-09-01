using System.Text;
using System.Text.Json;
using MockEMR.API.Models;

namespace MockEMR.API.Services
{
    public interface IWebhookService
    {
        Task SendPatientWebhookAsync(EmrPatient patient);
        Task SendProcedureWebhookAsync(EmrProcedure procedure);
    }

    public class WebhookService : IWebhookService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WebhookService> _logger;
        private readonly string _medTechWebhookUrl;

        public WebhookService(HttpClient httpClient, IConfiguration configuration, ILogger<WebhookService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _medTechWebhookUrl = configuration["MedTech:WebhookBaseUrl"] ?? "http://localhost:7001/api/webhook";
        }

        public async Task SendPatientWebhookAsync(EmrPatient patient)
        {
            try
            {
                var webhook = new
                {
                    patientId = patient.PatientId,
                    firstName = patient.FirstName,
                    lastName = patient.LastName,
                    email = patient.Email,
                    phone = patient.Phone,
                    dateOfBirth = patient.DateOfBirth,
                    source = "MockEMR",
                    eventTimestamp = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(webhook);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_medTechWebhookUrl}/emr/patient", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully sent patient webhook for {PatientId}", patient.PatientId);
                }
                else
                {
                    _logger.LogWarning("Failed to send patient webhook for {PatientId}. Status: {Status}",
                        patient.PatientId, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending patient webhook for {PatientId}", patient.PatientId);
            }
        }

        public async Task SendProcedureWebhookAsync(EmrProcedure procedure)
        {
            try
            {
                var webhook = new
                {
                    patientId = procedure.PatientId,
                    procedureId = procedure.ProcedureId,
                    procedureType = procedure.ProcedureType,
                    procedureCode = procedure.ProcedureCode,
                    procedureDate = procedure.ProcedureDate,
                    cost = procedure.Cost,
                    status = procedure.Status,
                    eventTimestamp = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(webhook);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_medTechWebhookUrl}/emr/procedure", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully sent procedure webhook for {ProcedureId}", procedure.ProcedureId);
                }
                else
                {
                    _logger.LogWarning("Failed to send procedure webhook for {ProcedureId}. Status: {Status}",
                        procedure.ProcedureId, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending procedure webhook for {ProcedureId}", procedure.ProcedureId);
            }
        }
    }
}
