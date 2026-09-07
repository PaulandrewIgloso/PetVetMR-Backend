using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetVetDB.Models
{
	[Table("Documents")]
	public class Documents
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DocumentID { get; set; }

		[ForeignKey("Pet")]
		public int PetID { get; set; }

		[Required]
		[MaxLength(255)]
		public string FileName { get; set; } = string.Empty;

		[Required]
		[MaxLength(500)]
		public string FilePath { get; set; } = string.Empty;
		public string? FileType { get; set; }
		public string? Description { get; set; }
		public string? DocumentType { get; set; }

		[ForeignKey("UploadedBy")]
		public int UploadedByUserID { get; set; }

		public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

		// Navigation
		public Pet Pet { get; set; } = null!;
		public User UploadedBy { get; set; } = null!;
	}
}