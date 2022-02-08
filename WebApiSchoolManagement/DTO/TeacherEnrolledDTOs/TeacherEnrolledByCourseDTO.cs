using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.TeacherEnrolledDTOs
{
    public class TeacherEnrolledByCourseDTO
    {
        public int id { get; set; }
        public string status { get; set; } = "Active";
        public int TeacherId { get; set; }
        public string name { get; set; }
        public string username { get; set; }

    }
}
