using Microsoft.AspNetCore.Mvc;
using MockEMR.API.Models;
using MockEMR.API.Services;

namespace MockEMR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmrController : ControllerBase
    {
        private readonly IWebhookService _webhookService;
        private readonly ILogger<EmrController> _logger;

        public EmrController(IWebhookService webhookService, ILogger<EmrController> logger)
        {
            _webhookService = webhookService;
            _logger = logger;
        }

        [HttpPost("patients")]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request)
        {
            var patient = new EmrPatient
            {
                PatientId = $"EMR_{DateTime.UtcNow:yyyyMMdd}_{Random.Shared.Next(1000, 9999)}",
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                DateOfBirth = request.DateOfBirth,
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Created patient in EMR: {PatientId}", patient.PatientId);

            // Send webhook to MedTech
            await _webhookService.SendPatientWebhookAsync(patient);

            return CreatedAtAction(nameof(GetPatient), new { id = patient.PatientId }, patient);
        }

        [HttpPost("procedures")]
        public async Task<IActionResult> RecordProcedure([FromBody] CreateProcedureRequest request)
        {
            var procedure = new EmrProcedure
            {
                ProcedureId = $"PROC_{DateTime.UtcNow:yyyyMMdd}_{Random.Shared.Next(10000, 99999)}",
                PatientId = request.PatientId,
                ProcedureType = request.ProcedureType,
                ProcedureCode = request.ProcedureCode,
                ProcedureDate = request.ProcedureDate,
                Cost = request.Cost,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Recorded procedure in EMR: {ProcedureId} for patient {PatientId}",
                procedure.ProcedureId, procedure.PatientId);

            // Send webhook to MedTech
            await _webhookService.SendProcedureWebhookAsync(procedure);

            return CreatedAtAction(nameof(GetProcedure), new { id = procedure.ProcedureId }, procedure);
        }

        [HttpGet("patients/{id}")]
        public IActionResult GetPatient(string id)
        {
            // For MVP, just return success - in real system this would query EMR database
            return Ok(new { patientId = id, status = "found" });
        }

        [HttpGet("procedures/{id}")]
        public IActionResult GetProcedure(string id)
        {
            return Ok(new { procedureId = id, status = "found" });
        }

        [HttpPost("simulate-batch")]
        public async Task<IActionResult> SimulateBatch()
        {
            var patients = GenerateSamplePatients(5);
            var procedures = new List<EmrProcedure>();

            foreach (var patient in patients)
            {
                _logger.LogInformation("Simulating patient creation: {PatientId}", patient.PatientId);
                await _webhookService.SendPatientWebhookAsync(patient);

                // Add some procedures for each patient
                var patientProcedures = GenerateSampleProcedures(patient.PatientId, 2);
                procedures.AddRange(patientProcedures);

                foreach (var procedure in patientProcedures)
                {
                    _logger.LogInformation("Simulating procedure: {ProcedureId}", procedure.ProcedureId);
                    await _webhookService.SendProcedureWebhookAsync(procedure);
                    await Task.Delay(500); // Small delay for demo effect
                }
            }

            return Ok(new
            {
                message = "Batch simulation completed",
                patientsCreated = patients.Count,
                proceduresCreated = procedures.Count,
                timestamp = DateTime.UtcNow
            });
        }

        private List<EmrPatient> GenerateSamplePatients(int count)
        {
            var names = new[]
            {
                ("John", "Smith"), ("Jane", "Doe"), ("Michael", "Johnson"),
                ("Sarah", "Williams"), ("David", "Brown"), ("Lisa", "Davis"),
                ("Robert", "Miller"), ("Maria", "Garcia"), ("James", "Wilson"),
                ("Jennifer", "Moore")
            };

            return Enumerable.Range(0, count)
                .Select(i =>
                {
                    var (firstName, lastName) = names[Random.Shared.Next(names.Length)];
                    return new EmrPatient
                    {
                        PatientId = $"EMR_{DateTime.UtcNow:yyyyMMdd}_{Random.Shared.Next(1000, 9999)}",
                        FirstName = firstName,
                        LastName = lastName,
                        Email = $"{firstName.ToLower()}.{lastName.ToLower()}@email.com",
                        Phone = $"555-{Random.Shared.Next(100, 999)}-{Random.Shared.Next(1000, 9999)}",
                        DateOfBirth = DateTime.Now.AddYears(-Random.Shared.Next(25, 65)),
                        CreatedAt = DateTime.UtcNow
                    };
                })
                .ToList();
        }

        private List<EmrProcedure> GenerateSampleProcedures(string patientId, int count)
        {
            var procedures = new[]
            {
                ("Botox Injection", "BOTOX", 450m),
                ("Dermal Filler", "FILLER", 650m),
                ("Laser Hair Removal", "LASER", 300m),
                ("HydraFacial", "FACIAL", 180m),
                ("CoolSculpting", "COOLSCULPT", 1200m)
            };

            return Enumerable.Range(0, count)
                .Select(i =>
                {
                    var (type, code, cost) = procedures[Random.Shared.Next(procedures.Length)];
                    return new EmrProcedure
                    {
                        ProcedureId = $"PROC_{DateTime.UtcNow:yyyyMMdd}_{Random.Shared.Next(10000, 99999)}",
                        PatientId = patientId,
                        ProcedureType = type,
                        ProcedureCode = code,
                        ProcedureDate = DateTime.Now.AddDays(-Random.Shared.Next(1, 30)),
                        Cost = cost + Random.Shared.Next(-50, 100), // Add some variation
                        Status = "Completed",
                        CreatedAt = DateTime.UtcNow
                    };
                })
                .ToList();
        }
    }
}
