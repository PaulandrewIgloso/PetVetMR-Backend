using AutoMapper;
using PetVetDB.Models;
using PetVetMR_ManagementSystem.DTOs.Appointment;
using PetVetMR_ManagementSystem.DTOs.Document;
using PetVetMR_ManagementSystem.DTOs.MedicalRecord;
using PetVetMR_ManagementSystem.DTOs.Pet;
using PetVetMR_ManagementSystem.DTOs.Role;
using PetVetMR_ManagementSystem.DTOs.User;
using PetVetMR_ManagementSystem.DTOs.Vaccination;

namespace PetVetMR_ManagementSystem.Mapping
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// ── Role ───────────────────────────────────────────────────────
			CreateMap<Role, RoleReadDto>();
			CreateMap<RoleCreateDto, Role>();
			CreateMap<RoleUpdateDto, Role>();

			// ── User ───────────────────────────────────────────────────────
			CreateMap<User, UserReadDto>()
				.ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.Role != null ? s.Role.RoleName : null));
			// Password is hashed separately in the service layer, not via AutoMapper.
			CreateMap<UserCreateDto, User>()
				.ForMember(d => d.PasswordHash, opt => opt.Ignore());
			CreateMap<UserUpdateDto, User>();

			// ── Pet ────────────────────────────────────────────────────────
			CreateMap<Pet, PetReadDto>()
				.ForMember(d => d.OwnerName, opt => opt.MapFrom(s => s.Owner != null ? $"{s.Owner.FirstName} {s.Owner.LastName}".Trim() : null));
			CreateMap<PetCreateDto, Pet>();
			CreateMap<PetUpdateDto, Pet>();

			// ── MedicalRecord ──────────────────────────────────────────────
			CreateMap<MedicalRecord, MedicalRecordReadDto>()
				.ForMember(d => d.PetName, opt => opt.MapFrom(s => s.Pet != null ? s.Pet.Name : null));
			CreateMap<MedicalRecordCreateDto, MedicalRecord>();
			CreateMap<MedicalRecordUpdateDto, MedicalRecord>();

			// ── Vaccination ────────────────────────────────────────────────
			CreateMap<Vaccination, VaccinationReadDto>()
				.ForMember(d => d.PetName, opt => opt.MapFrom(s => s.Pet != null ? s.Pet.Name : null))
				.ForMember(d => d.AdministeredByName, opt => opt.MapFrom(s => s.AdministeredBy != null ? $"{s.AdministeredBy.FirstName} {s.AdministeredBy.LastName}".Trim() : null));
			CreateMap<VaccinationCreateDto, Vaccination>();
			CreateMap<VaccinationUpdateDto, Vaccination>();

			// ── Document ───────────────────────────────────────────────────
			CreateMap<Documents, DocumentsReadDto>()
				.ForMember(d => d.PetName, opt => opt.MapFrom(s => s.Pet != null ? s.Pet.Name : null))
				.ForMember(d => d.UploadedByName, opt => opt.MapFrom(s => s.UploadedBy != null ? $"{s.UploadedBy.FirstName} {s.UploadedBy.LastName}".Trim() : null));
			CreateMap<DocumentUpdateDto, Documents>();

			// ── Appointment ────────────────────────────────────────────────
			CreateMap<Appointment, AppointmentReadDto>()
				.ForMember(d => d.PetName, opt => opt.MapFrom(s => s.Pet != null ? s.Pet.Name : null))
				.ForMember(d => d.BookedByName, opt => opt.MapFrom(s => s.BookedBy != null ? $"{s.BookedBy.FirstName} {s.BookedBy.LastName}".Trim() : null))
				.ForMember(d => d.VeterinarianName, opt => opt.MapFrom(s => s.Veterinarian != null ? $"{s.Veterinarian.FirstName} {s.Veterinarian.LastName}".Trim() : null))
				.ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
			CreateMap<AppointmentCreateDto, Appointment>();
			CreateMap<AppointmentUpdateDto, Appointment>();
		}
	}
}