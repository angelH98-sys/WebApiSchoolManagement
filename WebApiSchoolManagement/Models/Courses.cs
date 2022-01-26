using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.Models
{
    public class Courses
    {

        public int id { get; set; }
        [Required]
        public string coursename { get; set; }
        public string coursestatus { get; set; }
        [Required]
        [Range(1, 9, ErrorMessage = "Anio de curso debe ser entre {1} y {2}")]
        public int courseyear { get; set; }
        public List<Assignments> assignments { get; set; }
        public List<TeachersEnrolleds> teacherEnrolleds { get; set; }
    }
}
