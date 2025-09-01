using MedTech.Core.Handlers;
using MedTech.Core.Interfaces;
using MedTech.Core.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MedTech.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly GetPatientHandler _getPatientHandler;
        private readonly GetPatientHistoryHandler _getHistoryHandler;
        private readonly IPatientRepository _patientRepository;
        private readonly IProcedureRepository _procedureRepository;

        public PatientsController(
            GetPatientHandler getPatientHandler,
            GetPatientHistoryHandler getHistoryHandler,
            IPatientRepository patientRepository,
            IProcedureRepository procedureRepository)
        {
            _getPatientHandler = getPatientHandler;
            _getHistoryHandler = getHistoryHandler;
            _patientRepository = patientRepository;
            _procedureRepository = procedureRepository;
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentPatients()
        {
            var patients = await _patientRepository.GetRecentSyncsAsync(20);
            return Ok(patients);
        }

        [HttpGet("{emrPatientId}")]
        public async Task<IActionResult> GetPatient(string emrPatientId)
        {
            var result = await _getPatientHandler.HandleAsync(new GetPatientQuery(emrPatientId));

            if (!result.Found)
                return NotFound(new { error = "Patient not found" });

            return Ok(result.Patient);
        }

        [HttpGet("{emrPatientId}/history")]
        public async Task<IActionResult> GetPatientHistory(
            string emrPatientId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = new GetPatientHistoryQuery(emrPatientId, fromDate, toDate, page, pageSize);
            var result = await _getHistoryHandler.HandleAsync(query);

            return Ok(new
            {
                procedures = result.Procedures,
                totalCount = result.TotalCount,
                totalPoints = result.TotalPoints,
                page,
                pageSize
            });
        }

        [HttpGet("{emrPatientId}/points")]
        public async Task<IActionResult> GetPatientPoints(string emrPatientId)
        {
            var patientResult = await _getPatientHandler.HandleAsync(new GetPatientQuery(emrPatientId));

            if (!patientResult.Found)
                return NotFound(new { error = "Patient not found" });

            var totalPoints = await _procedureRepository.GetTotalPointsByPatientAsync(patientResult.Patient!.Id);

            return Ok(new
            {
                emrPatientId,
                MedTechPatientId = patientResult.Patient.MedTechPatientId,
                totalPoints,
                lastUpdated = patientResult.Patient.UpdatedAt
            });
        }
    }
}
