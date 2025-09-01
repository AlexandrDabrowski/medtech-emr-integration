using MedTech.Core.Interfaces;
using MedTech.Core.Models;
using MedTech.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedTech.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly MedTechContext _context;

        public PatientRepository(MedTechContext context)
        {
            _context = context;
        }

        public async Task<Patient?> GetByEmrIdAsync(string emrPatientId)
        {
            return await _context.Patients
                .Include(p => p.Procedures.OrderByDescending(pr => pr.ProcedureDate))
                .FirstOrDefaultAsync(p => p.EmrPatientId == emrPatientId);
        }

        public async Task<Patient?> GetByIdAsync(string medTechPatientId)
        {
            return await _context.Patients
                .Include(p => p.Procedures)
                .FirstOrDefaultAsync(p => p.MedTechPatientId == medTechPatientId);
        }

        public async Task<Patient> CreateAsync(Patient patient)
        {
            patient.MedTechPatientId = GenerateMedTechId();
            patient.CreatedAt = DateTime.UtcNow;
            patient.UpdatedAt = DateTime.UtcNow;

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<Patient> UpdateAsync(Patient patient)
        {
            patient.UpdatedAt = DateTime.UtcNow;
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<List<Patient>> GetRecentSyncsAsync(int count = 10)
        {
            return await _context.Patients
                .OrderByDescending(p => p.UpdatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string emrPatientId)
        {
            return await _context.Patients
                .AnyAsync(p => p.EmrPatientId == emrPatientId);
        }

        private string GenerateMedTechId()
        {
            return $"RMD_{DateTime.UtcNow:yyyyMMdd}_{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
    }
}
