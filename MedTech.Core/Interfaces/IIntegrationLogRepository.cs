using MedTech.Core.Models;

namespace MedTech.Core.Interfaces
{
    public interface IIntegrationLogRepository
    {
        Task LogAsync(string eventType, string status, string payload, string source, string? errorMessage = null, string? duration = null);
        Task<List<IntegrationLog>> GetRecentLogsAsync(int count = 50);
        Task<List<IntegrationLog>> GetErrorLogsAsync(DateTime? fromDate = null);
    }
}
