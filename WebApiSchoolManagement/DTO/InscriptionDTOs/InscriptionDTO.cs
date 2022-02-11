using WebApiSchoolManagement.DTO.CourseDTOs;
using WebApiSchoolManagement.DTO.StudentDTOs;
using WebApiSchoolManagement.DTO.TeacherDTOs;

namespace WebApiSchoolManagement.DTO.InscriptionDTOs
{
    public class InscriptionDTO
    {
        public int Id { get; set; }
        public decimal generalgrade { get; set; }
        public string status { get; set; }
        public int progress { get; set; }
        public decimal avarage { get; set; }
        public StudentDTO StudentDTO { get; set; }
        public CourseDTO CourseDTO { get; set; }
        public TeacherDTO TeacherDTO { get; set; }
    }
}
