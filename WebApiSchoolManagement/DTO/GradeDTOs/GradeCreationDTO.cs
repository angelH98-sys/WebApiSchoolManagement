using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.GradeDTOs
{
    public class GradeCreationDTO
    {
        [Required]
        [Range(0, 10, ErrorMessage = "Nota debe estar entre {1} y {2}")]
        public decimal grade { get; set; }
        [Required]
        public int idAssignment { get; set; }
        [Required]
        public int idInscription { get; set; }
    }
}
