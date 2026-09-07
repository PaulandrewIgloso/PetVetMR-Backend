using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PetVetDB.Models;
using PetVetMR_ManagementSystem.Database;
using PetVetMR_ManagementSystem.DTOs.MedicalRecord;

namespace PetVetMR_ManagementSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin,PetOwner")]
	public class MedicalRecordsController : ControllerBase
	{
		private readonly PetVetDbContext _context;
		private readonly IMapper _mapper;

		public MedicalRecordsController(PetVetDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		//Get all medicalrecords
		[HttpGet("GetAll")]
		public async Task<ActionResult<IEnumerable<MedicalRecordReadDto>>> GetAll()
		{
			var query = _context.MedicalRecords
				.AsNoTracking()
				.Include(r => r.Pet)
				.AsQueryable();

			if (!User.IsInRole("Admin"))
			{
				var currentUserId = GetCurrentUserId();
				if (currentUserId == null) return Unauthorized();
				query = query.Where(r => r.Pet.OwnerUserID == currentUserId.Value);
			}

			var records = await query
				.OrderByDescending(r => r.VisitDate)
				.ToListAsync();

			return Ok(_mapper.Map<List<MedicalRecordReadDto>>(records));
		}

		//Get medicalrecord by ID
		[HttpGet("GetBy{id:int}")]
		public async Task<ActionResult<MedicalRecordReadDto>> GetById(int id)
		{
			var record = await _context.MedicalRecords
				.AsNoTracking()
				.Include(r => r.Pet)
				.FirstOrDefaultAsync(r => r.RecordID == id);

			if (record == null) return NotFound();
			if (!User.IsInRole("Admin"))
			{
				var currentUserId = GetCurrentUserId();
				if (currentUserId == null || record.Pet.OwnerUserID != currentUserId.Value)
					return NotFound();
			}
			return Ok(_mapper.Map<MedicalRecordReadDto>(record));
		}

		//Create a new medicalrecord
		[HttpPost("Create")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<MedicalRecordReadDto>> Create(MedicalRecordCreateDto dto)
		{
			bool petExists = await _context.Pets.AnyAsync(p => p.PetID == dto.PetID);
			if (!petExists)
				return BadRequest("PetID does not reference an existing pet.");

			var record = _mapper.Map<MedicalRecord>(dto);
			record.CreatedAt = DateTime.UtcNow;
			record.UpdatedAt = null;

			_context.MedicalRecords.Add(record);
			await _context.SaveChangesAsync();

			var created = await _context.MedicalRecords
				.AsNoTracking()
				.Include(r => r.Pet)
				.FirstAsync(r => r.RecordID == record.RecordID);

			return CreatedAtAction(nameof(GetById), new { id = record.RecordID }, _mapper.Map<MedicalRecordReadDto>(created));
		}

		//Update an existing medicalrecord
		[HttpPut("Update{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<MedicalRecordReadDto>> Update(int id, MedicalRecordUpdateDto dto)
		{
			var record = await _context.MedicalRecords.FirstOrDefaultAsync(r => r.RecordID == id);
			if (record == null) return NotFound();

			_mapper.Map(dto, record);
			record.UpdatedAt = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			var updated = await _context.MedicalRecords
				.AsNoTracking()
				.Include(r => r.Pet)
				.FirstAsync(r => r.RecordID == id);

			return Ok(_mapper.Map<MedicalRecordReadDto>(updated));
		}

		//Delete a medicalrecord
		[HttpDelete("{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
		{
			var record = await _context.MedicalRecords.FirstOrDefaultAsync(r => r.RecordID == id);
			if (record == null) return NotFound();

			_context.MedicalRecords.Remove(record);
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
