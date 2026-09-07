using PetVetMR_ManagementSystem.DTOs.Auth;
using PetVetMR_ManagementSystem.DTOs.User;

namespace PetVetMR_ManagementSystem.Interface
{
	public interface IAuthService
	{
		Task<AuthResponseDto?> RegisterAsync(UserCreateDto dto);
		Task<AuthResponseDto?> LoginAsync(LoginDto dto);
	}
}