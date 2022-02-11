using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.GradeDTOs
{
    public class GradeCreationDTO
    {
        [Required]
        [Range(0, 10)]
        public decimal grade { get; set; }
        [Required]
        public int AssignmentId { get; set; }
        [Required]
        public int InscriptionId { get; set; }
    }
}
