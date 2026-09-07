using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PetVetDB.Models;
using PetVetMR_ManagementSystem.Database;
using PetVetMR_ManagementSystem.DTOs.Vaccination;

namespace PetVetMR_ManagementSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin,PetOwner")]
	public class VaccinationsController : ControllerBase
	{
		private readonly PetVetDbContext _context;
		private readonly IMapper _mapper;

		public VaccinationsController(PetVetDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		//Get all vaccinations 
		[HttpGet("GetAll")]
		public async Task<ActionResult<IEnumerable<VaccinationReadDto>>> GetAll()
		{
			var query = _context.Vaccinations
				.AsNoTracking()
				.Include(v => v.Pet)
				.Include(v => v.AdministeredBy)
				.AsQueryable();

			if (!User.IsInRole("Admin"))
			{
				var currentUserId = GetCurrentUserId();
				if (currentUserId == null) return Unauthorized();
				query = query.Where(v => v.Pet.OwnerUserID == currentUserId.Value);
			}

			var vaccinations = await query
				.OrderByDescending(v => v.VaccinationDate)
				.ToListAsync();

			return Ok(_mapper.Map<List<VaccinationReadDto>>(vaccinations));
		}

		//Get vaccination by ID
		[HttpGet("GetBy{id:int}")]
		public async Task<ActionResult<VaccinationReadDto>> GetById(int id)
		{
			var vaccination = await _context.Vaccinations
				.AsNoTracking()
				.Include(v => v.Pet)
				.Include(v => v.AdministeredBy)
				.FirstOrDefaultAsync(v => v.VaccinationID == id);

			if (vaccination == null) return NotFound();
			if (!User.IsInRole("Admin"))
			{
				var currentUserId = GetCurrentUserId();
				if (currentUserId == null || vaccination.Pet.OwnerUserID != currentUserId.Value)
					return NotFound();
			}
			return Ok(_mapper.Map<VaccinationReadDto>(vaccination));
		}

		//Create a new vaccination
		[HttpPost("Create")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<VaccinationReadDto>> Create(VaccinationCreateDto dto)
		{
			bool petExists = await _context.Pets.AnyAsync(p => p.PetID == dto.PetID);
			if (!petExists)
				return BadRequest("PetID does not reference an existing pet.");

			var currentUserId = GetCurrentUserId();
			if (currentUserId == null) return Unauthorized();

			var vaccination = _mapper.Map<Vaccination>(dto);
			vaccination.AdministeredByUserID = currentUserId.Value;
			vaccination.CreatedAt = DateTime.UtcNow;

			_context.Vaccinations.Add(vaccination);
			await _context.SaveChangesAsync();

			var created = await _context.Vaccinations
				.AsNoTracking()
				.Include(v => v.Pet)
				.Include(v => v.AdministeredBy)
				.FirstAsync(v => v.VaccinationID == vaccination.VaccinationID);

			return CreatedAtAction(nameof(GetById), new { id = vaccination.VaccinationID }, _mapper.Map<VaccinationReadDto>(created));
		}

		//Get all vaccinations for a specific pet
		[HttpPut("GetAllVaccineSpecificPet{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<VaccinationReadDto>> Update(int id, VaccinationUpdateDto dto)
		{
			var vaccination = await _context.Vaccinations.FirstOrDefaultAsync(v => v.VaccinationID == id);
			if (vaccination == null) return NotFound();

			_mapper.Map(dto, vaccination);
			await _context.SaveChangesAsync();

			var updated = await _context.Vaccinations
				.AsNoTracking()
				.Include(v => v.Pet)
				.Include(v => v.AdministeredBy)
				.FirstAsync(v => v.VaccinationID == id);

			return Ok(_mapper.Map<VaccinationReadDto>(updated));
		}

		[HttpDelete("{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
		{
			var vaccination = await _context.Vaccinations.FirstOrDefaultAsync(v => v.VaccinationID == id);
			if (vaccination == null) return NotFound();

			_context.Vaccinations.Remove(vaccination);
			await _context.SaveChangesAsync();
			return NoContent();
		}

		private int? GetCurrentUserId()
		{
			var subClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
				?? User.FindFirstValue(ClaimTypes.Name)
				?? User.FindFirstValue("sub");

			return int.TryParse(subClaim, out var id) ? id : null;
		}
	}
}
