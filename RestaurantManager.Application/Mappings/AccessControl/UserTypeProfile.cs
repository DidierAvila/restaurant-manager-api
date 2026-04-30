using AutoMapper;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Entities.AccessControl;
using System.Text.Json;

namespace RestaurantManager.Application.Mappings.AccessControl;

public class UserTypeProfile : Profile
{
    public UserTypeProfile()
    {
        // Entity to DTO mappings
        CreateMap<UserType, UserTypeDto>()
            .ForMember(dest => dest.AdditionalConfig, opt => opt.MapFrom(src => ParseAdditionalConfig(src.AdditionalConfig)));

        CreateMap<UserType, UserTypeDropdownDto>();

        CreateMap<UserType, UserTypeSummaryDto>()
            .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.Users.Count));

        CreateMap<UserType, UserTypeListResponseDto>()
            .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.Users.Count));

        // DTO to Entity mappings
        CreateMap<CreateUserTypeDto, UserType>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AdditionalConfig, opt => opt.MapFrom(SerializeCreateDtoAdditionalConfig));
        // AutoMapper mapea automáticamente: Name, Description, Status
        // AutoMapper ignora automáticamente: Users

        CreateMap<UpdateUserTypeDto, UserType>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.AdditionalConfig, opt => opt.MapFrom(SerializeUpdateDtoAdditionalConfig));
        // AutoMapper mapea automáticamente: Name, Description, Status
    }

    private static JsonElement? ParseAdditionalConfig(string? additionalConfig)
    {
        if (string.IsNullOrEmpty(additionalConfig))
            return null;

        try
        {
            using var document = JsonDocument.Parse(additionalConfig);
            return document.RootElement.Clone();
        }
        catch
        {
            return null;
        }
    }

    private static string? SerializeCreateDtoAdditionalConfig(CreateUserTypeDto src, UserType dest)
    {
        if (src.AdditionalConfig == null)
            return null;

        return JsonSerializer.Serialize(src.AdditionalConfig);
    }

    private static string? SerializeUpdateDtoAdditionalConfig(UpdateUserTypeDto src, UserType dest)
    {
        if (src.AdditionalConfig == null)
            return null;

        return JsonSerializer.Serialize(src.AdditionalConfig);
    }
}
