using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.AssignmentDTOs
{
    public class AssignmentCreationDTO
    {
        [Required]
        public string name { get; set; }
        [Required]
        [Range(1, 100)]
        public int coursevalue { get; set; }
    }
}
