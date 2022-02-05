using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.Models
{
    public class Grade
    {

        public int Id { get; set; }
        [Required]
        [Range(0,10)]
        public decimal grade { get; set; }
        public decimal gradevalue { get; set; }
        [Required]
        public int AssignmentId { get; set; }
        [Required]
        public int InscriptionId { get; set; }
        public Assignment Assignment { get; set; }
        public Inscription Inscription { get; set; }
    }
}
