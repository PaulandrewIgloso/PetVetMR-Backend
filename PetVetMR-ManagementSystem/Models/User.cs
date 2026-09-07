using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetVetDB.Models
{
	[Table("Users")]
	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int UserID { get; set; }

		[Required]
		[MaxLength(100)]
		public string Username { get; set; } = string.Empty;

		[Required]
		[MaxLength(255)]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		[MaxLength(255)]
		public string PasswordHash { get; set; } = string.Empty;

		[MaxLength(150)]
		public string? FirstName { get; set; }

		[MaxLength(150)]
		public string? LastName { get; set; }

		[MaxLength(20)]
		public string? Phone { get; set; }

		[ForeignKey("Role")]
		public int RoleID { get; set; }

		public bool IsActive { get; set; } = true;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// Navigation
		public Role Role { get; set; } = null!;

		public ICollection<Pet> OwnedPets { get; set; } = new List<Pet>();
		public ICollection<Vaccination> AdministeredVaccinations { get; set; } = new List<Vaccination>();
		public ICollection<Documents> UploadedDocuments { get; set; } = new List<Documents>();
		public ICollection<Appointment> BookedAppointments { get; set; } = new List<Appointment>();
		public ICollection<Appointment> VeterinarianAppointments { get; set; } = new List<Appointment>();
	}
}