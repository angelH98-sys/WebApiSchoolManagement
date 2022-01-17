using AutoMapper;
using WebApiSchoolManagement.DTO;
using WebApiSchoolManagement.Models;

namespace WebApiSchoolManagement.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //Teacher maps
            CreateMap<TeacherCreationDTO, Teachers>();
            CreateMap<TeacherCreationDTO, Users>();
            CreateMap<Teachers, TeacherDTO>()
                .ForMember(teacherDTO => teacherDTO.username, options => options.MapFrom(teacher => teacher.user.username))
                .ForMember(teacherDTO => teacherDTO.mail, options => options.MapFrom(teacher => teacher.user.mail));
        }
    }
}
