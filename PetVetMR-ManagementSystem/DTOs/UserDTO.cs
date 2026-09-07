using System.ComponentModel.DataAnnotations;

namespace PetVetMR_ManagementSystem.DTOs.User
{
	public class UserReadDto
	{
		public int UserID { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string? LastName { get; set; }
		public string? FirstName { get; set; }
		public string? Phone { get; set; }
		public int RoleID { get; set; }
		public string? RoleName { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedAt { get; set; }
	}

	public class UserCreateDto
	{
		[Required]
		[MaxLength(100)]
		public string Username { get; set; } = string.Empty;

		[Required]
		[MaxLength(255)]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		[MinLength(6)]
		public string Password { get; set; } = string.Empty;

		[MaxLength(150)]
		public string? LastName { get; set; }

		[MaxLength(150)]
		public string? FirstName { get; set; }

		[MaxLength(20)]
		public string? Phone { get; set; }

		[Required]
		public int RoleID { get; set; }

		public bool IsActive { get; set; } = true;
	}

	public class UserUpdateDto
	{
		[Required]
		[MaxLength(100)]
		public string Username { get; set; } = string.Empty;

		[Required]
		[MaxLength(255)]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[MaxLength(150)]
		public string? LastName { get; set; }

		[MaxLength(150)]
		public string? FirstName { get; set; }

		[MaxLength(20)]
		public string? Phone { get; set; }

		[Required]
		public int RoleID { get; set; }

		public bool IsActive { get; set; }
	}
}