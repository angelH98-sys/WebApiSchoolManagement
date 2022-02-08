using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
            CreateMap<TeacherCreationDTO, Teacher>();
            
            //->Read
            CreateMap<Teacher, TeacherDetailedDTO>()
                .ForMember(teacherDTO => teacherDTO.username, options => options.MapFrom(teacher => teacher.User.UserName))
                .ForMember(teacherDTO => teacherDTO.mail, options => options.MapFrom(teacher => teacher.User.Email));
            CreateMap<Teacher, TeacherDTO>()
                .ForMember(teacherDTO => teacherDTO.username, options => options.MapFrom(teacher => teacher.User.UserName));
            
            //->Patch
            CreateMap<Teacher, TeacherPatchDTO>()
                .ForMember(TeacherPatchDTO => TeacherPatchDTO.mail, options => options.MapFrom(teacher => teacher.User.Email))
                .ForMember(TeacherPatchDTO => TeacherPatchDTO.password, options => options.MapFrom(teacher => teacher.User.PasswordHash))
                .ReverseMap();
            

            //Student maps
            //->Create
            CreateMap<StudentCreationDTO, Student>();
            

            //->Read
            CreateMap<Student, StudentDTO>()
                .ForMember(studentDTO => studentDTO.username, options => options.MapFrom(student => student.User.UserName))
                .ForMember(studentDTO => studentDTO.mail, options => options.MapFrom(student => student.User.Email))
                .ReverseMap();
            
            //->Patch
            CreateMap<Student, StudentPatchDTO>()
                .ForMember(studentPatchDTO => studentPatchDTO.mail, options => options.MapFrom(student => student.User.Email))
                .ForMember(studentPatchDTO => studentPatchDTO.password, options => options.MapFrom(student => student.User.PasswordHash))
                .ReverseMap();
            
            //Course maps
            //->Create
            CreateMap<CourseCreationDTO, Course>();

            //->Read
            CreateMap<Course, CourseDetailedDTO>().ReverseMap();

            //->Patch
            CreateMap<CoursePatchDTO, Course>().ReverseMap();

            //Assignments Maps
            //->Read
            CreateMap<Assignment, AssignmentByCourseDTO>().ReverseMap();

            //->Create
            CreateMap<AssignmentCreationDTO, Assignment>().ReverseMap();

            //->Patch
            CreateMap<AssignmentPatchDTO, Assignment>().ReverseMap();


            //TeacherEnrolled Maps
            //->Read
            CreateMap<TeachersEnrolled, TeacherEnrolledByCourseDTO>()
                .ForMember(teacherEnrolledDTO => teacherEnrolledDTO.name, options => options.MapFrom(teacherEnrolled => teacherEnrolled.Teacher.name))
                .ForMember(teacherEnrolledDTO => teacherEnrolledDTO.username, options => options.MapFrom(teacherEnrolled => teacherEnrolled.Teacher.User.UserName));
            /*
            //TeacherEnrolled Maps
            //->Read
            CreateMap<Courses, CourseDetailedDTO>()
                .ForMember(courseDetailedDTO => courseDetailedDTO.assignments, options => options.MapFrom(MapAssignmentByCourse))
                .ForMember(courseDetailedDTO => courseDetailedDTO.teacherEnrolleds, options => options.MapFrom(MapTeacherEnrolledByCourse));

            //->Create
            CreateMap<CourseCreationDTO, Courses>().ReverseMap();

            

            //Inscriptions Maps
            //->Create
            CreateMap<InscriptionCreationDTO, Inscriptions>().ReverseMap();

            //Grades Maps
            //->Create
            CreateMap<Grades, GradeCreationDTO>().ReverseMap();*/
        }
        /*
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
        */
    }
}
