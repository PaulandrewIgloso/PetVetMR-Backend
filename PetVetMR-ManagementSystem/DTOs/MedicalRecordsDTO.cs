using System.ComponentModel.DataAnnotations;

namespace PetVetMR_ManagementSystem.DTOs.MedicalRecord
{
	public class MedicalRecordReadDto
	{
		public int RecordID { get; set; }
		public int PetID { get; set; }
		public string? PetName { get; set; }
		public DateTime VisitDate { get; set; }
		public string? Diagnosis { get; set; }
		public string? Treatment { get; set; }
		public string? Notes { get; set; }
		public string? Prescriptions { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}

	public class MedicalRecordCreateDto
	{
		[Required]
		public int PetID { get; set; }

		[Required]
		public DateTime VisitDate { get; set; }

		[MaxLength(500)]
		public string? Diagnosis { get; set; }

		public string? Treatment { get; set; }

		public string? Notes { get; set; }

		public string? Prescriptions { get; set; }
	}

	public class MedicalRecordUpdateDto
	{
		[Required]
		public DateTime VisitDate { get; set; }

		[MaxLength(500)]
		public string? Diagnosis { get; set; }

		public string? Treatment { get; set; }

		public string? Notes { get; set; }

		public string? Prescriptions { get; set; }
	}
}