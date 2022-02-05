using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.TeacherDTOs
{
    public class TeacherPatchDTO
    {
        [Required]
        public string teachername { get; set; }
        [Required]
        [EmailAddress]
        public string mail { get; set; }
    }
}
