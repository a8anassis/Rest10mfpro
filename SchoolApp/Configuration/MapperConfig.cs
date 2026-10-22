using AutoMapper;
using SchoolApp.DTO;
using SchoolApp.Models;

namespace SchoolApp.Configuration
{
    public class MapperConfig : Profile
    {

        public MapperConfig()
        {
            CreateMap<User, UserReadOnlyDTO>()
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.Role.Name));

            CreateMap<TeacherSignupDTO, User>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId!.Value));

            CreateMap<TeacherSignupDTO, Teacher>();

        }
    }
}
