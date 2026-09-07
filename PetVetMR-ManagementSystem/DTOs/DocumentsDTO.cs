using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PetVetMR_ManagementSystem.DTOs.Document
{
	public class DocumentsReadDto
	{
		public int DocumentID { get; set; }
		public int PetID { get; set; }
		public string? PetName { get; set; }
		public string FileName { get; set; } = string.Empty;
		public string FilePath { get; set; } = string.Empty;
		public string? FileType { get; set; }
		public string? Description { get; set; }
		public string? DocumentType { get; set; }
		public int UploadedByUserID { get; set; }
		public string? UploadedByName { get; set; }
		public DateTime UploadedAt { get; set; }
	}

	public class DocumentUpdateDto
	{
		[MaxLength(500)]
		public string? Description { get; set; }

		[MaxLength(100)]
		public string? DocumentType { get; set; }
	}

	public class DocumentUploadForm
	{
		[Required]
		public int PetID { get; set; }

		[Required]
		public IFormFile File { get; set; } = null!;

		public string? Description { get; set; }

		public string? DocumentType { get; set; }
	}
}