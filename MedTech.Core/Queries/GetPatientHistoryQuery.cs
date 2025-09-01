using MedTech.Core.Models;

namespace MedTech.Core.Queries
{
    public record GetPatientHistoryQuery(
        string EmrPatientId,
        DateTime? FromDate = null,
        DateTime? ToDate = null,
        int Page = 1,
        int PageSize = 50
    );

    public record GetPatientHistoryResult(
        List<Procedure> Procedures,
        int TotalCount,
        int TotalPoints
    );
}
