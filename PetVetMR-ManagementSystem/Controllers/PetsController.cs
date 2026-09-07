using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PetVetDB.Models;
using PetVetMR_ManagementSystem.Database;
using PetVetMR_ManagementSystem.DTOs.Pet;

namespace PetVetMR_ManagementSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin,PetOwner")]
	public class PetsController : ControllerBase
	{
		private readonly PetVetDbContext _context;
		private readonly IMapper _mapper;

		public PetsController(PetVetDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		//Get all pets — Admin sees all, PetOwner sees only their own
		[HttpGet("GetAll")]
		public async Task<ActionResult<IEnumerable<PetReadDto>>> GetAll()
		{
			var query = _context.Pets.AsNoTracking().Include(p => p.Owner).AsQueryable();

			if (!User.IsInRole("Admin"))
			{
				var currentUserId = GetCurrentUserId();
				if (currentUserId == null) return Unauthorized();
				query = query.Where(p => p.OwnerUserID == currentUserId.Value);
			}

			var pets = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
			return Ok(_mapper.Map<List<PetReadDto>>(pets));
		}

		//Get pet by ID — PetOwner can only see their own
		[HttpGet("GetBy{id:int}")]
		public async Task<ActionResult<PetReadDto>> GetById(int id)
		{
			var pet = await _context.Pets.AsNoTracking().Include(p => p.Owner).FirstOrDefaultAsync(p => p.PetID == id);
			if (pet == null) return NotFound();

			if (!User.IsInRole("Admin"))
			{
				var currentUserId = GetCurrentUserId();
				if (currentUserId == null || pet.OwnerUserID != currentUserId.Value)
					return NotFound();
			}

			return Ok(_mapper.Map<PetReadDto>(pet));
		}

		//Create a new pet — Admin can create for any owner; PetOwner can only create their own
		[HttpPost("Create")]
		[RequestSizeLimit(10_000_000)]
		public async Task<ActionResult<PetReadDto>> Create([FromForm] PetCreateForm form)
		{
			int ownerUserID;
			if (User.IsInRole("Admin"))
			{
				if (!form.OwnerUserID.HasValue)
					return BadRequest("OwnerUserID is required.");
				ownerUserID = form.OwnerUserID.Value;
			}
			else
			{
				var currentUserId = GetCurrentUserId();
				if (currentUserId == null) return Unauthorized();
				ownerUserID = currentUserId.Value; // PetOwner-submitted OwnerUserID is ignored on purpose
			}

			bool ownerExists = await _context.Users.AnyAsync(u => u.UserID == ownerUserID);
			if (!ownerExists)
				return BadRequest("OwnerUserID does not reference an existing user.");

			var pet = new Pet
			{
				Name = form.Name,
				Breed = form.Breed,
				Species = form.Species,
				DateOfBirth = form.DateOfBirth,
				Gender = form.Gender,
				Color = form.Color,
				MicrochipID = form.MicrochipID,
				OwnerUserID = ownerUserID,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
			};

			if (form.Photo != null && form.Photo.Length > 0)
			{
				var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Pets");
				Directory.CreateDirectory(uploadsFolder);

				var extension = Path.GetExtension(form.Photo.FileName);
				var storedFileName = $"{Guid.NewGuid()}{extension}";
				var fullPath = Path.Combine(uploadsFolder, storedFileName);

				using (var stream = new FileStream(fullPath, FileMode.Create))
				{
					await form.Photo.CopyToAsync(stream);
				}

				pet.PhotoPath = storedFileName;
			}

			_context.Pets.Add(pet);
			await _context.SaveChangesAsync();

			var created = await _context.Pets
				.AsNoTracking()
				.Include(p => p.Owner)
				.FirstAsync(p => p.PetID == pet.PetID);

			return CreatedAtAction(nameof(GetById), new { id = pet.PetID }, _mapper.Map<PetReadDto>(created));
		}

		//Get a pet's photo — PetOwner can only view their own pet's photo
		[HttpGet("Photo{id:int}")]
		public async Task<IActionResult> Photo(int id)
		{
			var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetID == id);
			if (pet == null || string.IsNullOrEmpty(pet.PhotoPath)) return NotFound();

			if (!User.IsInRole("Admin"))
			{
				var currentUserId = GetCurrentUserId();
				if (currentUserId == null || pet.OwnerUserID != currentUserId.Value)
					return NotFound();
			}

			var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Pets");
			var fullPath = Path.Combine(uploadsFolder, pet.PhotoPath);

			if (!System.IO.File.Exists(fullPath))
				return NotFound("Photo not found on server.");

			var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
			var contentType = pet.PhotoPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
				? "image/png"
				: "image/jpeg";

			return File(bytes, contentType);
		}

		//Update an existing pet — Admin only
		[HttpPut("Update{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<PetReadDto>> Update(int id, PetUpdateDto dto)
		{
			var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetID == id);
			if (pet == null) return NotFound();

			bool ownerExists = await _context.Users.AnyAsync(u => u.UserID == dto.OwnerUserID);
			if (!ownerExists)
				return BadRequest("OwnerUserID does not reference an existing user.");

			_mapper.Map(dto, pet);
			pet.UpdatedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			var updated = await _context.Pets
				.AsNoTracking()
				.Include(p => p.Owner)
				.FirstAsync(p => p.PetID == id);

			return Ok(_mapper.Map<PetReadDto>(updated));
		}

		//Delete a pet — Admin only
		[HttpDelete("{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
		{
			var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetID == id);
			if (pet == null) return NotFound();

			_context.Pets.Remove(pet);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				return Conflict("This pet has related records and cannot be deleted.");
			}

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