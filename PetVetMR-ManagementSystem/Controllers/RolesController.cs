using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetVetDB.Models;
using PetVetMR_ManagementSystem.Database;
using PetVetMR_ManagementSystem.DTOs.Role;

namespace PetVetMR_ManagementSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin")]
	public class RolesController : ControllerBase
	{
		private readonly PetVetDbContext _context;
		private readonly IMapper _mapper;

		public RolesController(PetVetDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		//Get all roles
		[HttpGet("Getall")]
		public async Task<ActionResult<IEnumerable<RoleReadDto>>> GetAll()
		{
			var roles = await _context.Roles.AsNoTracking().ToListAsync();
			return Ok(_mapper.Map<List<RoleReadDto>>(roles));
		}

		//Get role by ID
		[HttpGet("GetBy{id:int}")]
		public async Task<ActionResult<RoleReadDto>> GetById(int id)
		{
			var role = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.RoleID == id);
			if (role == null) return NotFound();

			return Ok(_mapper.Map<RoleReadDto>(role));
		}

		//Create a new role
		[HttpPost("Create")]
		public async Task<ActionResult<RoleReadDto>> Create(RoleCreateDto dto)
		{
			var roleName = dto.RoleName.Trim();
			bool exists = await _context.Roles.AnyAsync(r => r.RoleName == roleName);
			if (exists)
				return Conflict("A role with this name already exists.");

			var role = _mapper.Map<Role>(dto);
			role.RoleName = roleName;

			_context.Roles.Add(role);
			await _context.SaveChangesAsync();

			var readDto = _mapper.Map<RoleReadDto>(role);
			return CreatedAtAction(nameof(GetById), new { id = role.RoleID }, readDto);
		}

		//Update an existing role
		[HttpPut("Update{id:int}")]
		public async Task<ActionResult<RoleReadDto>> Update(int id, RoleUpdateDto dto)
		{
			var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleID == id);
			if (role == null) return NotFound();

			var roleName = dto.RoleName.Trim();
			bool nameTaken = await _context.Roles.AnyAsync(r => r.RoleID != id && r.RoleName == roleName);
			if (nameTaken)
				return Conflict("A role with this name already exists.");

			_mapper.Map(dto, role);
			role.RoleName = roleName;
			await _context.SaveChangesAsync();

			return Ok(_mapper.Map<RoleReadDto>(role));
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
		{
			var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleID == id);
			if (role == null) return NotFound();

			_context.Roles.Remove(role);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				return Conflict("This role is currently in use and cannot be deleted.");
			}

			return NoContent();
		}
	}
}
