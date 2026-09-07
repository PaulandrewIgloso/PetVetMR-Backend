using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PetVetDB.Models;
using PetVetMR_ManagementSystem.Database;
using PetVetMR_ManagementSystem.DTOs.Document;

namespace PetVetMR_ManagementSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin,PetOwner")]
	public class DocumentsController : ControllerBase
	{
		private readonly PetVetDbContext _context;
		private readonly IMapper _mapper;

		public DocumentsController(PetVetDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		//Get all documents
		[HttpGet("GetAll")]
		public async Task<ActionResult<IEnumerable<DocumentsReadDto>>> GetAll()
		{
			var query = _context.Documents
				.AsNoTracking()
				.Include(d => d.Pet)
				.Include(d => d.UploadedBy)
				.AsQueryable();

			if (!User.IsInRole("Admin"))
			{
				var currentUserId = GetCurrentUserId();
				if (currentUserId == null) return Unauthorized();
				query = query.Where(d => d.Pet.OwnerUserID == currentUserId.Value);
			}

			var docs = await query
				.OrderByDescending(d => d.UploadedAt)
				.ToListAsync();

			return Ok(_mapper.Map<List<DocumentsReadDto>>(docs));
		}

		//Get document by ID
		[HttpGet("GetBy{id:int}")]
		public async Task<ActionResult<DocumentsReadDto>> GetById(int id)
		{
			var doc = await _context.Documents
				.AsNoTracking()
				.Include(d => d.Pet)
				.Include(d => d.UploadedBy)
				.FirstOrDefaultAsync(d => d.DocumentID == id);

			if (doc == null) return NotFound();
			if (!User.IsInRole("Admin"))
			{
				var currentUserId = GetCurrentUserId();
				if (currentUserId == null || doc.Pet.OwnerUserID != currentUserId.Value)
					return NotFound();
			}
			return Ok(_mapper.Map<DocumentsReadDto>(doc));
		}

		//Create a new document
		[HttpPost("Create")]
		[Authorize(Roles = "Admin")]
		[RequestSizeLimit(20_000_000)]
		public async Task<ActionResult<DocumentsReadDto>> Create([FromForm] DocumentUploadForm form)
		{
			if (form.File == null || form.File.Length == 0)
				return BadRequest("A file is required.");

			bool petExists = await _context.Pets.AnyAsync(p => p.PetID == form.PetID);
			if (!petExists)
				return BadRequest("PetID does not reference an existing pet.");

			var currentUserId = GetCurrentUserId();
			if (currentUserId == null) return Unauthorized();

			var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Documents");
			Directory.CreateDirectory(uploadsFolder);

			var extension = Path.GetExtension(form.File.FileName);
			var storedFileName = $"{Guid.NewGuid()}{extension}";
			var fullPath = Path.Combine(uploadsFolder, storedFileName);

			using (var stream = new FileStream(fullPath, FileMode.Create))
			{
				await form.File.CopyToAsync(stream);
			}

			var doc = new Documents
			{
				PetID = form.PetID,
				FileName = form.File.FileName,
				FilePath = storedFileName,
				FileType = form.File.ContentType,
				Description = form.Description,
				DocumentType = form.DocumentType,
				UploadedByUserID = currentUserId.Value,
				UploadedAt = DateTime.UtcNow,
			};

			_context.Documents.Add(doc);
			await _context.SaveChangesAsync();

			var created = await _context.Documents
				.AsNoTracking()
				.Include(d => d.Pet)
				.Include(d => d.UploadedBy)
				.FirstAsync(d => d.DocumentID == doc.DocumentID);

			return CreatedAtAction(nameof(GetById), new { id = doc.DocumentID }, _mapper.Map<DocumentsReadDto>(created));
		}

		//Download the actual file for a document
		[HttpGet("Download{id:int}")]
		public async Task<IActionResult> Download(int id)
		{
			var doc = await _context.Documents.Include(d => d.Pet).FirstOrDefaultAsync(d => d.DocumentID == id);
			if (doc == null) return NotFound();

			if (!User.IsInRole("Admin"))
			{
				var currentUserId = GetCurrentUserId();
				if (currentUserId == null || doc.Pet.OwnerUserID != currentUserId.Value)
					return NotFound();
			}

			var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Documents");
			var fullPath = Path.Combine(uploadsFolder, doc.FilePath);

			if (!System.IO.File.Exists(fullPath))
				return NotFound("File not found on server.");

			var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
			return File(bytes, doc.FileType ?? "application/octet-stream", doc.FileName);
		}

		//Update an existing document
		[HttpPut("Update{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<DocumentsReadDto>> Update(int id, DocumentUpdateDto dto)
		{
			var doc = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentID == id);
			if (doc == null) return NotFound();

			_mapper.Map(dto, doc);
			await _context.SaveChangesAsync();

			var updated = await _context.Documents
				.AsNoTracking()
				.Include(d => d.Pet)
				.Include(d => d.UploadedBy)
				.FirstAsync(d => d.DocumentID == id);

			return Ok(_mapper.Map<DocumentsReadDto>(updated));
		}

		//Delete a document
		[HttpDelete("{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
		{
			var doc = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentID == id);
			if (doc == null) return NotFound();

			_context.Documents.Remove(doc);
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
