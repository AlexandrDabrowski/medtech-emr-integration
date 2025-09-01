using MedTech.Core.Commands;
using MedTech.Core.Models;
using MedTech.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedTech.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IIntegrationService _integrationService;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IIntegrationService integrationService, ILogger<WebhookController> logger)
        {
            _integrationService = integrationService;
            _logger = logger;
        }

        [HttpPost("emr/patient")]
        public async Task<IActionResult> PatientWebhook([FromBody] EmrPatientWebhook webhook)
        {
            try
            {
                _logger.LogInformation("Received patient webhook for EMR ID: {EmrPatientId}", webhook.PatientId);

                var command = new CreatePatientCommand(
                    webhook.PatientId,
                    webhook.FirstName,
                    webhook.LastName,
                    webhook.Email,
                    webhook.Phone ?? string.Empty,
                    webhook.DateOfBirth,
                    webhook.Source ?? "EMR"
                );

                var result = await _integrationService.SyncPatientAsync(command);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        MedTechPatientId = result.PatientId,
                        message = result.ErrorMessage ?? "Patient synced successfully"
                    });
                }

                return BadRequest(new { success = false, error = result.ErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing patient webhook");
                return StatusCode(500, new { success = false, error = "Internal server error" });
            }
        }

        [HttpPost("emr/procedure")]
        public async Task<IActionResult> ProcedureWebhook([FromBody] EmrProcedureWebhook webhook)
        {
            try
            {
                _logger.LogInformation("Received procedure webhook for patient: {PatientId}, procedure: {ProcedureId}",
                    webhook.PatientId, webhook.ProcedureId);

                var command = new RecordProcedureCommand(
                    webhook.PatientId,
                    webhook.ProcedureId,
                    webhook.ProcedureType,
                    webhook.ProcedureCode,
                    webhook.ProcedureDate,
                    webhook.Cost,
                    Enum.Parse<Core.Models.ProcedureStatus>(webhook.Status, true)
                );

                var result = await _integrationService.ProcessProcedureAsync(command);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        procedureId = result.ProcedureId,
                        pointsEarned = result.PointsEarned,
                        message = "Procedure recorded successfully"
                    });
                }

                return BadRequest(new { success = false, error = result.ErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing procedure webhook");
                return StatusCode(500, new { success = false, error = "Internal server error" });
            }
        }

        [HttpPost("test")]
        public IActionResult TestWebhook([FromBody] object payload)
        {
            _logger.LogInformation("Test webhook received: {Payload}", payload);
            return Ok(new { success = true, message = "Webhook endpoint is working", timestamp = DateTime.UtcNow });
        }
    }
}
