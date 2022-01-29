using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.InscriptionDTOs
{
    public class InscriptionCreationDTO
    {
        [Required]
        public int idStudent { get; set; }
        [Required]
        public int idCourse { get; set; }
        [Required]
        public int idTeacher { get; set; }
    }
}