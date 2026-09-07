using System.ComponentModel.DataAnnotations;

namespace PetVetMR_ManagementSystem.DTOs.Appointment
{
	public class AppointmentReadDto
	{
		public int AppointmentID { get; set; }
		public int PetID { get; set; }
		public string? PetName { get; set; }
		public DateTime AppointmentDateTime { get; set; }
		public string? Reason { get; set; }
		public string Status { get; set; } = string.Empty;
		public string? Notes { get; set; }
		public string? BookedByName { get; set; }
		public int? VeterinarianUserID { get; set; }
		public string? VeterinarianName { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}

	public class AppointmentCreateDto
	{
		[Required]
		public int PetID { get; set; }

		[Required]
		public DateTime AppointmentDateTime { get; set; }

		[MaxLength(300)]
		public string? Reason { get; set; }

		public int? VeterinarianUserID { get; set; }

		public string? Notes { get; set; }
	}

	public class AppointmentUpdateDto
	{
		[Required]
		public DateTime AppointmentDateTime { get; set; }

		[MaxLength(300)]
		public string? Reason { get; set; }

		[Required]
		[MaxLength(50)]
		public string Status { get; set; } = "Scheduled";

		public int? VeterinarianUserID { get; set; }

		public string? Notes { get; set; }
	}
}