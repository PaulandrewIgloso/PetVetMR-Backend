using Microsoft.AspNetCore.Mvc;
using PetVetMR_ManagementSystem.DTOs.Auth;
using PetVetMR_ManagementSystem.DTOs.User;
using PetVetMR_ManagementSystem.Interface;

namespace PetVetMR_ManagementSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		public async Task<ActionResult<AuthResponseDto>> Register(UserCreateDto dto)
		{
			var result = await _authService.RegisterAsync(dto);
			if (result == null)
				return Conflict("A user with that email or username already exists.");

			return Ok(result);
		}

		[HttpPost("login")]
		public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
		{
			var result = await _authService.LoginAsync(dto);
			if (result == null)
				return Unauthorized("Invalid email or password.");

			return Ok(result);
		}
	}
}