using AutoMapper;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Entities.AccessControl;
using System.Text.Json;

namespace RestaurantManager.Application.Mappings.AccessControl;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // Entity to DTO mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.AdditionalData, opt => opt.Ignore())
            .ForMember(dest => dest.UserTypeName, opt => opt.Ignore()) // Se asignará manualmente
            .AfterMap((src, dest) => {
                if (!string.IsNullOrEmpty(src.ExtraData) && src.ExtraData != "{}")
                {
                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        dest.AdditionalData = JsonSerializer.Deserialize<Dictionary<string, object>>(src.ExtraData, options);
                    }
                    catch
                    {
                        dest.AdditionalData = [];
                    }
                }
                else
                {
                    dest.AdditionalData = [];
                }
            });

        CreateMap<User, UserWithDetailsDto>()
            .ForMember(dest => dest.AdditionalData, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
            .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType))
            .AfterMap((src, dest) => {
                if (!string.IsNullOrEmpty(src.ExtraData) && src.ExtraData != "{}")
                {
                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        dest.AdditionalData = JsonSerializer.Deserialize<Dictionary<string, object>>(src.ExtraData, options);
                    }
                    catch
                    {
                        dest.AdditionalData = [];
                    }
                }
                else
                {
                    dest.AdditionalData = [];
                }
            });

        CreateMap<User, UserSummaryDto>()
            .ForMember(dest => dest.UserTypeName, opt => opt.MapFrom(src => src.UserType!.Name != null ? src.UserType.Name : string.Empty))
            .ForMember(dest => dest.RoleCount, opt => opt.MapFrom(src => src.Roles.Count))
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore()); // Se puede mapear desde Sessions si es necesario

        // DTO to Entity mappings

        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Roles, opt => opt.Ignore())
            .ForMember(dest => dest.Sessions, opt => opt.Ignore())
            .ForMember(dest => dest.UserType, opt => opt.Ignore())
            .ForMember(dest => dest.ExtraData, opt => opt.Ignore());

        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.ExtraData, opt => opt.MapFrom(src => "{}"))
            .ForMember(dest => dest.Sessions, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore())
            .ForMember(dest => dest.UserType, opt => opt.Ignore())
            .AfterMap((src, dest) => {
                if (src.AdditionalData != null)
                {
                    dest.ExtraData = JsonSerializer.Serialize(src.AdditionalData);
                }
            });
        // AutoMapper mapea automáticamente: Email, Password, Image, Phone, UserTypeId
        // AutoMapper ignora automáticamente: Sessions, Roles, UserType

        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.Sessions, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore())
            .ForMember(dest => dest.UserType, opt => opt.Ignore())
            .ForSourceMember(src => src.RoleIds, opt => opt.DoNotValidate()) // Ignorar RoleIds ya que se maneja por separado
            .AfterMap((src, dest) => {
                if (src.AdditionalData != null)
                {
                    dest.ExtraData = JsonSerializer.Serialize(src.AdditionalData);
                }
            });
        // AutoMapper mapea automáticamente: Image, Phone, UserTypeId
        // AutoMapper ignora automáticamente: Id, Email, Password, CreatedAt, Sessions, Roles, UserType

        // Mapeo optimizado sin AdditionalData para listas
        CreateMap<User, UserBasicDto>()
            .ForMember(dest => dest.UserTypeName, opt => opt.MapFrom(src => src.UserType != null ? src.UserType : null))
            .ForMember(dest => dest.FirstRoleName, opt => opt.MapFrom(src => src.Roles != null && src.Roles.Any() ? src.Roles.First().Name : null));
    }
}
