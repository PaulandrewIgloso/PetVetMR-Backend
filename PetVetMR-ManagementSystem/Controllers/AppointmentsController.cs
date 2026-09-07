using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PetVetDB.Models;
using PetVetMR_ManagementSystem.Database;
using PetVetMR_ManagementSystem.DTOs.Appointment;

namespace PetVetMR_ManagementSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin,PetOwner")]
	public class AppointmentsController : ControllerBase
	{
		private readonly PetVetDbContext _context;
		private readonly IMapper _mapper;

		public AppointmentsController(PetVetDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		//Get all appointments
		//Get all appointments
		[HttpGet("GetAll")]
		public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> GetAll()
		{
			try
			{
				var query = _context.Appointments
					.AsNoTracking()
					.Include(a => a.Pet)
					.Include(a => a.BookedBy)
					.Include(a => a.Veterinarian)
					.AsQueryable();

				if (!User.IsInRole("Admin"))
				{
					var currentUserId = GetCurrentUserId();
					if (currentUserId == null) return Unauthorized();
					query = query.Where(a => a.Pet.OwnerUserID == currentUserId.Value);
				}

				var appointments = await query
					.OrderByDescending(a => a.AppointmentDateTime)
					.ToListAsync();

				return Ok(_mapper.Map<List<AppointmentReadDto>>(appointments));
			}
			catch (Exception ex)
			{
				// TEMPORARY — remove after debugging
				return StatusCode(500, new
				{
					error = ex.Message,
					stackTrace = ex.StackTrace,
					inner = ex.InnerException?.Message
				});
			}
		}

		//Get appointment by ID
		[HttpGet("GetBy{id:int}")]
		public async Task<ActionResult<AppointmentReadDto>> GetById(int id)
		{
			var appointment = await _context.Appointments
				.AsNoTracking()
				.Include(a => a.Pet)
				.Include(a => a.BookedBy)
				.Include(a => a.Veterinarian)
				.FirstOrDefaultAsync(a => a.AppointmentID == id);

			if (appointment == null) return NotFound();
			if (!User.IsInRole("Admin") && !User.IsInRole("Staff"))
			{
				var currentUserId = GetCurrentUserId();
				if (currentUserId == null || appointment.Pet.OwnerUserID != currentUserId.Value)
					return NotFound();
			}
			return Ok(_mapper.Map<AppointmentReadDto>(appointment));
		}

		//Create a new appointment — Admin can book for any pet, PetOwner only for their own
		[HttpPost("Create")]
		public async Task<ActionResult<AppointmentReadDto>> Create(AppointmentCreateDto dto)
		{
			var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetID == dto.PetID);
			if (pet == null)
				return BadRequest("PetID does not reference an existing pet.");

			var currentUserId = GetCurrentUserId();
			if (currentUserId == null) return Unauthorized();

			if (!User.IsInRole("Admin") && pet.OwnerUserID != currentUserId.Value)
				return Forbid();

			if (!User.IsInRole("Admin"))
				dto.VeterinarianUserID = null;

			if (dto.VeterinarianUserID.HasValue && !await _context.Users.AnyAsync(u => u.UserID == dto.VeterinarianUserID.Value))
				return BadRequest("VeterinarianUserID does not reference an existing user.");

			var appointment = _mapper.Map<Appointment>(dto);
			appointment.BookedByUserID = currentUserId.Value;
			appointment.Status = AppointmentStatus.Scheduled;
			appointment.CreatedAt = DateTime.UtcNow;
			appointment.UpdatedAt = DateTime.UtcNow;

			_context.Appointments.Add(appointment);
			await _context.SaveChangesAsync();

			var created = await _context.Appointments
				.AsNoTracking()
				.Include(a => a.Pet)
				.Include(a => a.BookedBy)
				.Include(a => a.Veterinarian)
				.FirstAsync(a => a.AppointmentID == appointment.AppointmentID);

			return CreatedAtAction(nameof(GetById), new { id = appointment.AppointmentID }, _mapper.Map<AppointmentReadDto>(created));
		}

		//Update an existing appointment
		[HttpPut("Update{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<AppointmentReadDto>> Update(int id, AppointmentUpdateDto dto)
		{
			var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.AppointmentID == id);
			if (appointment == null) return NotFound();

			if (dto.VeterinarianUserID.HasValue && !await _context.Users.AnyAsync(u => u.UserID == dto.VeterinarianUserID.Value))
				return BadRequest("VeterinarianUserID does not reference an existing user.");

			if (!Enum.TryParse<AppointmentStatus>(dto.Status, true, out var parsedStatus))
				return BadRequest("Status must be one of: Scheduled, Completed, Cancelled, NoShow.");

			_mapper.Map(dto, appointment);
			appointment.Status = parsedStatus;
			appointment.UpdatedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			var updated = await _context.Appointments
				.AsNoTracking()
				.Include(a => a.Pet)
				.Include(a => a.BookedBy)
				.Include(a => a.Veterinarian)
				.FirstAsync(a => a.AppointmentID == id);

			return Ok(_mapper.Map<AppointmentReadDto>(updated));
		}

		//Delete an appointment
		[HttpDelete("{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
		{
			var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.AppointmentID == id);
			if (appointment == null) return NotFound();

			_context.Appointments.Remove(appointment);
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