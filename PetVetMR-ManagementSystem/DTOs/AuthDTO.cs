using System.ComponentModel.DataAnnotations;

namespace PetVetMR_ManagementSystem.DTOs.Auth
{
	public class LoginDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		public string Password { get; set; } = string.Empty;
	}

	public class AuthResponseDto
	{
		public string Token { get; set; } = string.Empty;
		public DateTime ExpiresAt { get; set; }
		public int UserID { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string? RoleName { get; set; }
	}
}