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
                .ForMember(teacherDTO => teacherDTO.mail, options => options.MapFrom(teacher => teacher.User.Email));
            CreateMap<Teacher, TeacherDTO>();
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
            CreateMap<Course, CourseDTO>().ReverseMap();

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


            //Inscription Maps
            //->Create
            CreateMap<Inscription, InscriptionCreationDTO>().ReverseMap();

            //->Read
            CreateMap<Inscription, InscriptionDTO>()
                .ForMember(inscriptionDTO => inscriptionDTO.TeacherDTO, options => options.MapFrom(MapInscriptionDTOTeacherDTO))
                .ForMember(inscriptionDTO => inscriptionDTO.CourseDTO, options => options.MapFrom(MapInscriptionDTOCourseDTO))
                .ForMember(inscriptionDTO => inscriptionDTO.StudentDTO, options => options.MapFrom(MapInscriptionDTOStudentDTO));

            //Grades Maps
            //->Create
            CreateMap<Grade, GradeCreationDTO>().ReverseMap();
            
        }

        private StudentDTO MapInscriptionDTOStudentDTO(Inscription inscription, InscriptionDTO inscriptionDTO)
        {
            var student = inscription.Student;

            if (student == null) 
            {
                return null;
            }

            return new StudentDTO()
            {
                Id = student.Id,
                name = student.name
            };
        }

        private CourseDTO MapInscriptionDTOCourseDTO(Inscription inscription, InscriptionDTO inscriptionDTO)
        {
            var course = inscription.Course;

            if (course == null) 
            {
                return null;
            }

            return new CourseDTO()
            {
                Id = course.Id,
                name = course.name,
                status = course.status
            };
        }

        private TeacherDTO MapInscriptionDTOTeacherDTO(Inscription inscription, InscriptionDTO inscriptionDTO)
        {
            var teacher = inscription.Teacher;

            if (teacher == null) 
            {
                return null;
            }

            return new TeacherDTO()
            {
                Id = teacher.Id,
                name = teacher.name
            };
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
