using System.ComponentModel.DataAnnotations;
using WebApiSchoolManagement.DTO.AssignmentDTOs;
using WebApiSchoolManagement.DTO.TeacherEnrolledDTOs;
using WebApiSchoolManagement.Models;

namespace WebApiSchoolManagement.DTO.CourseDTOs
{
    public class CourseDetailedDTO
    {
        public int id { get; set; }
        public string coursename { get; set; }
        public string coursestatus { get; set; }
        public int courseyear { get; set; }
        public List<AssignmentByCourseDTO> assignments { get; set; }
        public List<TeacherEnrolledByCourseDTO> teacherEnrolleds { get; set; }
    }
}
