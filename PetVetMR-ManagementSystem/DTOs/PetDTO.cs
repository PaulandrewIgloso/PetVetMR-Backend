using System.ComponentModel.DataAnnotations;

namespace PetVetMR_ManagementSystem.DTOs.Pet
{
	public class PetReadDto
	{
		public int PetID { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Breed { get; set; }
		public string Species { get; set; } = string.Empty;
		public DateOnly? DateOfBirth { get; set; }
		public string? Gender { get; set; }
		public string? Color { get; set; }
		public string? MicrochipID { get; set; }
		public string? PhotoPath { get; set; }
		public int OwnerUserID { get; set; }
		public string? OwnerName { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}

	public class PetCreateDto
	{
		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(100)]
		public string? Breed { get; set; }

		[MaxLength(50)]
		public string Species { get; set; }

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

		[Required]
		public int OwnerUserID { get; set; }
	}

	public class PetUpdateDto
	{
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

		[Required]
		public int OwnerUserID { get; set; }
	}

	public class PetCreateForm
	{
		public string Name { get; set; } = string.Empty;
		public string? Breed { get; set; }
		public string Species { get; set; } = "Dog";
		public DateOnly? DateOfBirth { get; set; }
		public string? Gender { get; set; }
		public string? Color { get; set; }
		public string? MicrochipID { get; set; }

		public int? OwnerUserID { get; set; }

		public IFormFile? Photo { get; set; }
	}
}