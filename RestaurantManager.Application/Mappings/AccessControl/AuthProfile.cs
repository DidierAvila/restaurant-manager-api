using AutoMapper;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Entities.AccessControl;

namespace RestaurantManager.Application.Mappings.AccessControl;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        // AutoMapper mapea automáticamente propiedades con nombres iguales
        CreateMap<Role, RoleDto>();
        CreateMap<Permission, PermissionDto>();
        CreateMap<Session, SessionDto>();
        
        // Mapeo personalizado para UserRole -> UserRoleDto
        // Mapea usando la navigation property Role
        CreateMap<UserRole, UserRoleDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Role.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Role.Description))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Role.Status));

        // Mapeo personalizado para UserRole -> RoleDropdownDto (optimizado para GetById)
        // Solo mapea Id y Name para mejor rendimiento
        CreateMap<UserRole, RoleDropdownDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Role.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Role.Name));
    }
}
