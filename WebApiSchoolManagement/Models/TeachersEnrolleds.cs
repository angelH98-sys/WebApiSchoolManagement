using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiSchoolManagement.Models
{
    public class TeachersEnrolleds
    {

        public int id { get; set; }
        [Required]
        public string enrolledstatus { get; set; } = "Active";
        [Required]
        public int idTeacher { get; set; }
        [Required]
        public int idCourse { get; set; }
        [ForeignKey("idTeacher")]
        public Teachers teacher { get; set; }
        [ForeignKey("idCourse")]
        public Courses course { get; set; }
    }
}
