using MedTech.Core.Interfaces;
using MedTech.Core.Models;
using MedTech.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedTech.Infrastructure.Repositories
{
    public class IntegrationLogRepository(MedTechContext context) : IIntegrationLogRepository
    {
        private readonly MedTechContext _context = context;

        public async Task LogAsync(string eventType, string status, string payload, string source, string? errorMessage = null, string? duration = null)
        {
            var log = new IntegrationLog
            {
                Timestamp = DateTime.UtcNow,
                EventType = eventType,
                Status = status,
                Payload = payload,
                Source = source,
                ErrorMessage = errorMessage,
                ProcessingDuration = duration
            };

            _context.IntegrationLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<IntegrationLog>> GetRecentLogsAsync(int count = 50)
        {
            return await _context.IntegrationLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<IntegrationLog>> GetErrorLogsAsync(DateTime? fromDate = null)
        {
            var query = _context.IntegrationLogs
                .Where(l => l.Status.Contains("Error") || l.Status.Contains("Failed"));

            if (fromDate.HasValue)
                query = query.Where(l => l.Timestamp >= fromDate.Value);

            return await query
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }
    }
}
