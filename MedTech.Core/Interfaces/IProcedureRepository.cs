using MedTech.Core.Models;

namespace MedTech.Core.Interfaces
{
    public interface IProcedureRepository
    {
        Task<Procedure> CreateAsync(Procedure procedure);
        Task<List<Procedure>> GetByPatientIdAsync(int patientId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<Procedure>> GetRecentProceduresAsync(int count = 20);
        Task<bool> ExistsAsync(string emrReferenceId);
        Task<int> GetTotalPointsByPatientAsync(int patientId);
    }
}
