using MedTech.Core.Models;

namespace MedTech.Core.Queries
{
    public record GetPatientQuery(string EmrPatientId);

    public record GetPatientResult(
        Patient? Patient,
        bool Found
    );
}
