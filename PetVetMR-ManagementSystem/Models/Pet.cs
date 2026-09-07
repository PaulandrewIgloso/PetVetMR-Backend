using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetVetDB.Models
{
	[Table("Pets")]
	public class Pet
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int PetID { get; set; }

		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(100)]
		public string? Breed { get; set; }

		[MaxLength(50)]
		public string Species { get; set; } = "Dog";

		public DateOnly? DateOfBirth { get; set; }

		[MaxLength(1)]
		[RegularExpression("^[MFN]$", ErrorMessage = "Gender must be M, F, or N")]
		public string? Gender { get; set; }

		[MaxLength(50)]
		public string? Color { get; set; }

		[MaxLength(50)]
		public string? MicrochipID { get; set; }

		[MaxLength(500)]
		public string? PhotoPath { get; set; }

		[ForeignKey("Owner")]
		public int OwnerUserID { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		// Navigation
		public User Owner { get; set; } = null!;

		public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
		public ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
		public ICollection<Documents> Documents { get; set; } = new List<Documents>();
		public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
	}
}