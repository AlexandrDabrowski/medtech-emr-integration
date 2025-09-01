using MedTech.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedTech.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IProcedureRepository _procedureRepository;
        private readonly IIntegrationLogRepository _logRepository;

        public DashboardController(
            IPatientRepository patientRepository,
            IProcedureRepository procedureRepository,
            IIntegrationLogRepository logRepository)
        {
            _patientRepository = patientRepository;
            _procedureRepository = procedureRepository;
            _logRepository = logRepository;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var recentPatients = await _patientRepository.GetRecentSyncsAsync(10);
            var recentProcedures = await _procedureRepository.GetRecentProceduresAsync(10);
            var recentLogs = await _logRepository.GetRecentLogsAsync(20);
            var errorLogs = await _logRepository.GetErrorLogsAsync(DateTime.UtcNow.AddDays(-7));

            return Ok(new
            {
                summary = new
                {
                    totalPatients = recentPatients.Count,
                    recentProcedures = recentProcedures.Count,
                    errorCount = errorLogs.Count,
                    lastSyncTime = recentLogs.FirstOrDefault()?.Timestamp
                },
                recentPatients = recentPatients.Select(p => new
                {
                    p.EmrPatientId,
                    p.MedTechPatientId,
                    name = $"{p.FirstName} {p.LastName}",
                    p.SyncStatus,
                    p.UpdatedAt
                }),
                recentProcedures = recentProcedures.Select(p => new
                {
                    p.EmrReferenceId,
                    patientName = $"{p.Patient.FirstName} {p.Patient.LastName}",
                    p.ProcedureType,
                    p.PointsEarned,
                    p.ProcedureDate
                }),
                recentLogs = recentLogs.Select(l => new
                {
                    l.Timestamp,
                    l.EventType,
                    l.Status,
                    l.Source,
                    l.ErrorMessage,
                    l.ProcessingDuration
                })
            });
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetIntegrationLogs([FromQuery] int count = 50)
        {
            var logs = await _logRepository.GetRecentLogsAsync(count);
            return Ok(logs);
        }

        [HttpGet("errors")]
        public async Task<IActionResult> GetErrorLogs([FromQuery] int days = 7)
        {
            var fromDate = DateTime.UtcNow.AddDays(-days);
            var errors = await _logRepository.GetErrorLogsAsync(fromDate);
            return Ok(errors);
        }
    }
}
