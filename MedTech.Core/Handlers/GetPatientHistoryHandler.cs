using MedTech.Core.Interfaces;
using MedTech.Core.Models;
using MedTech.Core.Queries;

namespace MedTech.Core.Handlers
{
    public class GetPatientHistoryHandler
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IProcedureRepository _procedureRepository;

        public GetPatientHistoryHandler(IPatientRepository patientRepository, IProcedureRepository procedureRepository)
        {
            _patientRepository = patientRepository;
            _procedureRepository = procedureRepository;
        }

        public async Task<GetPatientHistoryResult> HandleAsync(GetPatientHistoryQuery query)
        {
            var patient = await _patientRepository.GetByEmrIdAsync(query.EmrPatientId);
            if (patient == null)
            {
                return new GetPatientHistoryResult(new List<Procedure>(), 0, 0);
            }

            var procedures = await _procedureRepository.GetByPatientIdAsync(
                patient.Id,
                query.FromDate,
                query.ToDate
            );

            // Simple pagination for MVP
            var paginatedProcedures = procedures
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var totalPoints = await _procedureRepository.GetTotalPointsByPatientAsync(patient.Id);

            return new GetPatientHistoryResult(paginatedProcedures, procedures.Count, totalPoints);
        }
    }
}
