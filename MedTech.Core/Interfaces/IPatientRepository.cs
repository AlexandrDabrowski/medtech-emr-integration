using MedTech.Core.Models;

namespace MedTech.Core.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient?> GetByEmrIdAsync(string emrPatientId);
        Task<Patient?> GetByIdAsync(string medTechPatientId);
        Task<Patient> CreateAsync(Patient patient);
        Task<Patient> UpdateAsync(Patient patient);
        Task<List<Patient>> GetRecentSyncsAsync(int count = 10);
        Task<bool> ExistsAsync(string emrPatientId);
    }
}
