using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.StudentDTOs
{
    public class StudentPatchDTO
    {
        [Required]
        public string studentname { get; set; }
        [Required]
        [Range(1, 9)]
        public int courseyear { get; set; }
        [Required]
        [EmailAddress]
        public string mail { get; set; }
    }
}
