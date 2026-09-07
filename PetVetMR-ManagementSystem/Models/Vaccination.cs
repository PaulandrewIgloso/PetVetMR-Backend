using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetVetDB.Models
{
	[Table("Vaccinations")]
	public class Vaccination
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int VaccinationID { get; set; }

		[ForeignKey("Pet")]
		public int PetID { get; set; }

		[Required]
		[MaxLength(150)]
		public string VaccineType { get; set; } = string.Empty;

		[Required]
		public DateOnly VaccinationDate { get; set; }

		[MaxLength(100)]
		public string? BatchNumber { get; set; }

		public DateOnly? NextDueDate { get; set; }

		[ForeignKey("AdministeredBy")]
		public int AdministeredByUserID { get; set; }

		[MaxLength(500)]
		public string? Notes { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// Navigation
		public Pet Pet { get; set; } = null!;
		public User AdministeredBy { get; set; } = null!;
	}
}