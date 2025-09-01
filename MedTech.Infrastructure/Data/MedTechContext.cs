using MedTech.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MedTech.Infrastructure.Data
{
    public class MedTechContext(DbContextOptions<MedTechContext> options) : DbContext(options)
    {
        public DbSet<Patient> Patients { get; set; } = null!;
        public DbSet<Procedure> Procedures { get; set; } = null!;
        public DbSet<IntegrationLog> IntegrationLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.EmrPatientId).IsUnique();
                entity.HasIndex(e => e.MedTechPatientId).IsUnique();
                entity.HasIndex(e => e.Email);

                entity.Property(e => e.EmrPatientId).HasMaxLength(100).IsRequired();
                entity.Property(e => e.MedTechPatientId).HasMaxLength(100).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.SyncStatus).HasConversion<string>();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<Procedure>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.EmrReferenceId).IsUnique();
                entity.HasIndex(e => new { e.PatientId, e.ProcedureDate });

                entity.Property(e => e.EmrReferenceId).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ProcedureType).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ProcedureCode).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Cost).HasPrecision(10, 2);

                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Patient)
                      .WithMany(e => e.Procedures)
                      .HasForeignKey(e => e.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<IntegrationLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => new { e.EventType, e.Status });

                entity.Property(e => e.EventType).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Source).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Payload).HasColumnType("nvarchar(max)");
                entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
                entity.Property(e => e.ProcessingDuration).HasMaxLength(50);
            });
        }
    }
}
