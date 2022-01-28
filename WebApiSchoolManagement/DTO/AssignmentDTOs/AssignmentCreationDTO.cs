using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.AssignmentDTOs
{
    public class AssignmentCreationDTO
    {
        [Required]
        public string assignmentname { get; set; }
        [Required]
        [Range(1, 100, 
            ErrorMessage = "Valor del curso debe ser entre {1} y {2}")]
        public int coursevalue { get; set; }
    }
}
