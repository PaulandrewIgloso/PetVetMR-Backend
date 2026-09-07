using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetVetDB.Models
{
	[Table("Roles")]
	public class Role
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int RoleID { get; set; }

		[Required]
		[MaxLength(50)]
		public string RoleName { get; set; } = string.Empty;

		[MaxLength(200)]
		public string? Description { get; set; }

		// Navigation
		public ICollection<User> Users { get; set; } = new List<User>();
	}
}