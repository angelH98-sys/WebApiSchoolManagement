using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.Models
{
    public class Course
    {

        public int Id { get; set; }
        [Required]
        public string name { get; set; }
        public string status { get; set; }
        [Required]
        [Range(1, 9)]
        public int courseyear { get; set; }
        public List<Assignment> Assignments { get; set; }
        public List<TeachersEnrolled> TeacherEnrolleds { get; set; }
    }
}
