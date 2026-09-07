using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetVetDB.Models
{
	public enum AppointmentStatus
	{
		Scheduled,
		Completed,
		Cancelled,
		NoShow
	}

	[Table("Appointments")]
	public class Appointment
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int AppointmentID { get; set; }

		[ForeignKey("Pet")]
		public int PetID { get; set; }

		[Required]
		public DateTime AppointmentDateTime { get; set; }

		[MaxLength(300)]
		public string? Reason { get; set; }

		[Required]
		[MaxLength(50)]
		public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

		public string? Notes { get; set; }

		[ForeignKey("BookedBy")]
		public int BookedByUserID { get; set; }

		[ForeignKey("Veterinarian")]
		public int? VeterinarianUserID { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		// Navigation
		public Pet Pet { get; set; } = null!;
		public User BookedBy { get; set; } = null!;
		public User? Veterinarian { get; set; }
	}
}