using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.AssignmentDTOs
{
    public class AssignmentPatchDTO : AssignmentCreationDTO
    {
        [Required]
        public string status { get; set; }
    }
}
