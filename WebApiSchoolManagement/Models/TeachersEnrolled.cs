using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiSchoolManagement.Models
{
    public class TeachersEnrolled
    {

        public int id { get; set; }
        [Required]
        public string status { get; set; } = "Active";
        [Required]
        public int TeacherId { get; set; }
        [Required]
        public int CourseId { get; set; }
        public Teacher Teacher { get; set; }
        public Course Course { get; set; }
    }
}
