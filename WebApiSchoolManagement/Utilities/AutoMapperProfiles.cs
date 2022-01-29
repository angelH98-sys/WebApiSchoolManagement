using AutoMapper;
using WebApiSchoolManagement.DTO.AssignmentDTOs;
using WebApiSchoolManagement.DTO.CourseDTOs;
using WebApiSchoolManagement.DTO.GradeDTOs;
using WebApiSchoolManagement.DTO.InscriptionDTOs;
using WebApiSchoolManagement.DTO.StudentDTOs;
using WebApiSchoolManagement.DTO.TeacherDTOs;
using WebApiSchoolManagement.DTO.TeacherEnrolledDTOs;
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


            //Student maps
            //->Create
            CreateMap<StudentCreationDTO, Students>();
            CreateMap<StudentCreationDTO, Users>();

            //->Read
            CreateMap<Students, StudentDTO>()
                .ForMember(studentDTO => studentDTO.username, options => options.MapFrom(student => student.user.username))
                .ForMember(studentDTO => studentDTO.mail, options => options.MapFrom(student => student.user.mail))
                .ReverseMap();

            //->Patch
            CreateMap<Students, StudentPatchDTO>()
                .ForMember(studentPatchDTO => studentPatchDTO.mail, options => options.MapFrom(student => student.user.mail))
                .ReverseMap();
            CreateMap<Users, StudentPatchDTO>().ReverseMap();


            //TeacherEnrolled Maps
            //->Read
            CreateMap<Courses, CourseDetailedDTO>()
                .ForMember(courseDetailedDTO => courseDetailedDTO.assignments, options => options.MapFrom(MapAssignmentByCourse))
                .ForMember(courseDetailedDTO => courseDetailedDTO.teacherEnrolleds, options => options.MapFrom(MapTeacherEnrolledByCourse));

            //->Create
            CreateMap<CourseCreationDTO, Courses>().ReverseMap();

            //Assignments Maps
            //->Read
            CreateMap<Assignments, AssignmentByCourseDTO>();

            //->Create
            CreateMap<AssignmentCreationDTO, Assignments>().ReverseMap();

            //Inscriptions Maps
            //->Create
            CreateMap<InscriptionCreationDTO, Inscriptions>().ReverseMap();

            //Grades Maps
            //->Create
            CreateMap<Grades, GradeCreationDTO>().ReverseMap();
        }

        private List<AssignmentByCourseDTO> MapAssignmentByCourse(Courses course, CourseDetailedDTO courseDetailedDTO)
        {

            if (course.assignments == null) 
            {
                return null;
            }

            var result = new List<AssignmentByCourseDTO>();

            foreach (Assignments assgn in course.assignments) 
            {
                result.Add(new AssignmentByCourseDTO 
                {
                    id = assgn.id,
                    assignmentname = assgn.assignmentname,
                    coursevalue = assgn.coursevalue,
                    assignmentstatus = assgn.assignmentstatus
                });
            }

            return result;
        }

        private List<TeacherEnrolledByCourseDTO> MapTeacherEnrolledByCourse(Courses course, CourseDetailedDTO courseDetailedDTO)
        {
            if (course.teacherEnrolleds == null) 
            {
                return null;
            }

            var result = new List<TeacherEnrolledByCourseDTO>();

            foreach (TeachersEnrolleds tchenr in course.teacherEnrolleds)
            {

                result.Add(new TeacherEnrolledByCourseDTO 
                {
                    id = tchenr.id,
                    enrolledstatus = tchenr.enrolledstatus,
                    idTeacher = tchenr.idTeacher,
                    teachername = tchenr.teacher.teachername,
                    username = tchenr.teacher.user.username
                });;
            }

            return result;
        }
    }
}
