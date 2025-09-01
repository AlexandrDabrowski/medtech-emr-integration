using MedTech.Core.Interfaces;
using MedTech.Core.Queries;

namespace MedTech.Core.Handlers
{
    public class GetPatientHandler
    {
        private readonly IPatientRepository _patientRepository;

        public GetPatientHandler(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<GetPatientResult> HandleAsync(GetPatientQuery query)
        {
            var patient = await _patientRepository.GetByEmrIdAsync(query.EmrPatientId);
            return new GetPatientResult(patient, patient != null);
        }
    }
}
