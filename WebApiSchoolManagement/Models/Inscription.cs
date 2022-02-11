using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.Models
{
    public class Inscription
    {

        public int Id { get; set; }
        [Range(0, 10)]
        public decimal generalgrade { get; set; } = 0;
        public string status { get; set; } = "Active";
        [Range(0, 100)]
        public int progress { get; set; } = 0;
        [Range(0, 10)]
        public decimal avarage { get; set; } = 0;
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int CourseId { get; set; }
        [Required]
        public int TeacherId { get; set; }
        public Student Student { get; set; }
        public Course Course { get; set; }
        public Teacher Teacher { get; set; }

    }
}
