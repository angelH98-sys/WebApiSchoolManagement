using AutoMapper;
using WebApiSchoolManagement.DTO.TeacherDTOs;
using WebApiSchoolManagement.Models;

namespace WebApiSchoolManagement.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //Teacher maps
            //->Create
            CreateMap<TeacherCreationDTO, Teachers>();
            CreateMap<TeacherCreationDTO, Users>();

            //->Read
            CreateMap<Teachers, TeacherDTO>()
                .ForMember(teacherDTO => teacherDTO.username, options => options.MapFrom(teacher => teacher.user.username))
                .ForMember(teacherDTO => teacherDTO.mail, options => options.MapFrom(teacher => teacher.user.mail));
            
            //->Patch
            CreateMap<Teachers, TeacherPatchDTO>()
                .ForMember(TeacherPatchDTO => TeacherPatchDTO.mail, options => options.MapFrom(teacher => teacher.user.mail))
                .ReverseMap();
            CreateMap<Users, TeacherPatchDTO>().ReverseMap();
        }
    }
}
