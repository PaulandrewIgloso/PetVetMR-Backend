using System.ComponentModel.DataAnnotations;

namespace PetVetMR_ManagementSystem.DTOs.Vaccination
{
	public class VaccinationReadDto
	{
		public int VaccinationID { get; set; }
		public int PetID { get; set; }
		public string? PetName { get; set; }
		public string VaccineType { get; set; } = string.Empty;
		public DateOnly VaccinationDate { get; set; }
		public string? BatchNumber { get; set; }
		public DateOnly? NextDueDate { get; set; }
		public int AdministeredByUserID { get; set; }
		public string? AdministeredByName { get; set; }
		public string? Notes { get; set; }
		public DateTime CreatedAt { get; set; }
	}

	public class VaccinationCreateDto
	{
		[Required]
		public int PetID { get; set; }

		[Required]
		[MaxLength(150)]
		public string VaccineType { get; set; } = string.Empty;

		[Required]
		public DateOnly VaccinationDate { get; set; }

		[MaxLength(100)]
		public string? BatchNumber { get; set; }

		public DateOnly? NextDueDate { get; set; }

		[MaxLength(500)]
		public string? Notes { get; set; }
	}

	public class VaccinationUpdateDto
	{
		[Required]
		[MaxLength(150)]
		public string VaccineType { get; set; } = string.Empty;

		[Required]
		public DateOnly VaccinationDate { get; set; }

		[MaxLength(100)]
		public string? BatchNumber { get; set; }

		public DateOnly? NextDueDate { get; set; }

		[MaxLength(500)]
		public string? Notes { get; set; }
	}
}