using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.TeacherEnrolledDTOs
{
    public class TeacherEnrolledByCourseDTO
    {
        public int id { get; set; }
        public string enrolledstatus { get; set; } = "Active";
        public int idTeacher { get; set; }
        public string teachername { get; set; }
        public string username { get; set; }

    }
}
