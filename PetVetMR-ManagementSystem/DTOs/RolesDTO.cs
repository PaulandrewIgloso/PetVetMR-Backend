using System.ComponentModel.DataAnnotations;

namespace PetVetMR_ManagementSystem.DTOs.Role
{
	public class RoleReadDto
	{
		public int RoleID { get; set; }
		public string RoleName { get; set; } = string.Empty;
		public string? Description { get; set; }
	}

	public class RoleCreateDto
	{
		[Required]
		[MaxLength(50)]
		public string RoleName { get; set; } = string.Empty;

		[MaxLength(200)]
		public string? Description { get; set; }
	}

	public class RoleUpdateDto
	{
		[Required]
		[MaxLength(50)]
		public string RoleName { get; set; } = string.Empty;

		[MaxLength(200)]
		public string? Description { get; set; }
	}
}