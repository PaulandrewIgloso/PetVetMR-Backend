using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetVetDB.Models
{
	[Table("MedicalRecords")]
	public class MedicalRecord
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int RecordID { get; set; }

		[ForeignKey("Pet")]
		public int PetID { get; set; }

		[Required]
		public DateTime VisitDate { get; set; }

		[MaxLength(500)]
		public string? Diagnosis { get; set; }

		public string? Treatment { get; set; }

		public string? Notes { get; set; }

		public string? Prescriptions { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedAt { get; set; }

		// Navigation
		public Pet Pet { get; set; } = null!;
	}
}