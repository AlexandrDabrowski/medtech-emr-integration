using MedTech.Core.Interfaces;
using MedTech.Core.Models;
using MedTech.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedTech.Infrastructure.Repositories
{
    public class ProcedureRepository : IProcedureRepository
    {
        private readonly MedTechContext _context;

        public ProcedureRepository(MedTechContext context)
        {
            _context = context;
        }

        public async Task<Procedure> CreateAsync(Procedure procedure)
        {
            procedure.CreatedAt = DateTime.UtcNow;
            _context.Procedures.Add(procedure);
            await _context.SaveChangesAsync();
            return procedure;
        }

        public async Task<List<Procedure>> GetByPatientIdAsync(int patientId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Procedures
                .Where(p => p.PatientId == patientId);

            if (fromDate.HasValue)
                query = query.Where(p => p.ProcedureDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.ProcedureDate <= toDate.Value);

            return await query
                .OrderByDescending(p => p.ProcedureDate)
                .ToListAsync();
        }

        public async Task<List<Procedure>> GetRecentProceduresAsync(int count = 20)
        {
            return await _context.Procedures
                .Include(p => p.Patient)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string emrReferenceId)
        {
            return await _context.Procedures
                .AnyAsync(p => p.EmrReferenceId == emrReferenceId);
        }

        public async Task<int> GetTotalPointsByPatientAsync(int patientId)
        {
            return await _context.Procedures
                .Where(p => p.PatientId == patientId && p.Status == ProcedureStatus.Completed)
                .SumAsync(p => p.PointsEarned);
        }
    }
}
