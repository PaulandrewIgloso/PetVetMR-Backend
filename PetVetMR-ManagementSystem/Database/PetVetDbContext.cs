using Microsoft.EntityFrameworkCore;
using PetVetDB.Models;

namespace PetVetMR_ManagementSystem.Database
{
	public class PetVetDbContext : DbContext
	{
		public PetVetDbContext(DbContextOptions<PetVetDbContext> options) : base(options) { }

		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<Pet> Pets { get; set; }
		public DbSet<MedicalRecord> MedicalRecords { get; set; }
		public DbSet<Vaccination> Vaccinations { get; set; }
		public DbSet<Documents> Documents { get; set; }
		public DbSet<Appointment> Appointments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Pet>().HasIndex(p => p.OwnerUserID);

			// Pet -> User (Owner)
			modelBuilder.Entity<Pet>()
				.HasIndex(p => p.MicrochipID)
				.IsUnique()
				.HasFilter("[MicrochipID] IS NOT NULL");

			// Vaccination -> User (AdministeredBy)
			modelBuilder.Entity<Vaccination>()
				.HasOne(v => v.AdministeredBy)
				.WithMany(u => u.AdministeredVaccinations)
				.HasForeignKey(v => v.AdministeredByUserID)
				.OnDelete(DeleteBehavior.Restrict);

			// Documents -> User (UploadedBy)
			modelBuilder.Entity<Documents>()
				.HasOne(d => d.UploadedBy)
				.WithMany(u => u.UploadedDocuments)
				.HasForeignKey(d => d.UploadedByUserID)
				.OnDelete(DeleteBehavior.Restrict);

			// Appointment -> User (BookedBy / Veterinarian)
			modelBuilder.Entity<Appointment>()
						.Property(a => a.Status)
						.HasConversion<string>();

			// Appointment -> User (BookedBy / Veterinarian)
			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.BookedBy)
				.WithMany(u => u.BookedAppointments)
				.HasForeignKey(a => a.BookedByUserID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.Veterinarian)
				.WithMany(u => u.VeterinarianAppointments)
				.HasForeignKey(a => a.VeterinarianUserID)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}