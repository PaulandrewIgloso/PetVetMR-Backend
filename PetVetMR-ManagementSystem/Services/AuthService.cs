using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PetVetDB.Models;
using PetVetMR_ManagementSystem.Database;
using PetVetMR_ManagementSystem.DTOs.Auth;
using PetVetMR_ManagementSystem.DTOs.User;
using PetVetMR_ManagementSystem.Interface;

namespace PetVetMR_ManagementSystem.Services
{
	public class AuthService : IAuthService
	{
		private readonly PetVetDbContext _context;
		private readonly IMapper _mapper;
		private readonly IConfiguration _config;
		private readonly PasswordHasher<User> _passwordHasher = new();

		public AuthService(PetVetDbContext context, IMapper mapper, IConfiguration config)
		{
			_context = context;
			_mapper = mapper;
			_config = config;
		}

		public async Task<AuthResponseDto?> RegisterAsync(UserCreateDto dto)
		{
			bool exists = await _context.Users.AnyAsync(u => u.Email == dto.Email || u.Username == dto.Username);
			if (exists) return null;

			var user = _mapper.Map<User>(dto);
			user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

			_context.Users.Add(user);
			await _context.SaveChangesAsync();
			await _context.Entry(user).Reference(u => u.Role).LoadAsync();

			return GenerateAuthResponse(user);
		}

		public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
		{
			var user = await _context.Users
				.Include(u => u.Role)
				.FirstOrDefaultAsync(u => u.Email == dto.Email);

			if (user == null || !user.IsActive) return null;

			var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
			if (result == PasswordVerificationResult.Failed) return null;

			await _context.SaveChangesAsync();

			return GenerateAuthResponse(user);
		}

		private AuthResponseDto GenerateAuthResponse(User user)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var issuedAt = DateTime.UtcNow;

			var claims = new List<Claim>
			{
				new(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
				new(JwtRegisteredClaimNames.Email, user.Email),
				new(ClaimTypes.Name, user.Username),
				new(ClaimTypes.Role, user.Role?.RoleName ?? string.Empty),
				new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(issuedAt).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
			};

			var expires = issuedAt.AddHours(8);

			var token = new JwtSecurityToken(
				claims: claims,
				notBefore: issuedAt,
				expires: expires,
				signingCredentials: creds);

			return new AuthResponseDto
			{
				Token = new JwtSecurityTokenHandler().WriteToken(token),
				ExpiresAt = expires,
				UserID = user.UserID,
				Username = user.Username,
				Email = user.Email,
				RoleName = user.Role?.RoleName
			};
		}
	}
}