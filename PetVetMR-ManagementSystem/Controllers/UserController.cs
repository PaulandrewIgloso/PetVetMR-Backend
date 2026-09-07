using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetVetMR_ManagementSystem.Database;
using PetVetMR_ManagementSystem.DTOs.User;

namespace PetVetMR_ManagementSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin")]
	public class UsersController : ControllerBase
	{
		private readonly PetVetDbContext _context;
		private readonly IMapper _mapper;

		public UsersController(PetVetDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		[HttpGet("GetAll")]
		public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll()
		{
			var users = await _context.Users.Include(u => u.Role).ToListAsync();
			return Ok(_mapper.Map<List<UserReadDto>>(users));
		}

		[HttpGet("GetBy{id}")]
		public async Task<ActionResult<UserReadDto>> GetById(int id)
		{
			var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserID == id);
			if (user == null) return NotFound();
			return Ok(_mapper.Map<UserReadDto>(user));
		}

		[HttpPut("Update{id:int}")]
		public async Task<ActionResult<UserReadDto>> Update(int id, UserUpdateDto dto)
		{
			var user = await _context.Users.FindAsync(id);
			if (user == null) return NotFound();

			bool roleExists = await _context.Roles.AnyAsync(r => r.RoleID == dto.RoleID);
			if (!roleExists) return BadRequest("RoleID does not reference an existing role.");

			bool emailTaken = await _context.Users.AnyAsync(u => u.UserID != id && u.Email == dto.Email);
			if (emailTaken) return Conflict("Another user already has that email.");

			user.Username = dto.Username;
			user.Email = dto.Email;
			user.FirstName = dto.FirstName;
			user.LastName = dto.LastName;
			user.Phone = dto.Phone;
			user.RoleID = dto.RoleID;
			user.IsActive = dto.IsActive;

			await _context.SaveChangesAsync();

			var updated = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserID == id);
			return Ok(_mapper.Map<UserReadDto>(updated));
		}
	}
}