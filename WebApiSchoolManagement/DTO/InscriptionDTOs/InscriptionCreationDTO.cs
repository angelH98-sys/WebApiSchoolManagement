using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.InscriptionDTOs
{
    public class InscriptionCreationDTO
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int CourseId { get; set; }
        [Required]
        public int TeacherId { get; set; }
    }
}