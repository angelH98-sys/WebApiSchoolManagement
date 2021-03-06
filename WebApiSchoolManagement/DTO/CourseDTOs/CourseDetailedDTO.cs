using System.ComponentModel.DataAnnotations;
using WebApiSchoolManagement.DTO.AssignmentDTOs;
using WebApiSchoolManagement.DTO.TeacherEnrolledDTOs;
using WebApiSchoolManagement.Models;

namespace WebApiSchoolManagement.DTO.CourseDTOs
{
    public class CourseDetailedDTO
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public int courseyear { get; set; }
        public List<AssignmentByCourseDTO> assignments { get; set; }
        public List<TeacherEnrolledByCourseDTO> teacherEnrolleds { get; set; }
    }
}
