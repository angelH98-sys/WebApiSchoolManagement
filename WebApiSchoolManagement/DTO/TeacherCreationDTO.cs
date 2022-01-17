using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO
{
    public class TeacherCreationDTO
    {
        [Required]
        public string teachername { get; set; }
        [Required]
        public string psswd { get; set; }
        [Required]
        public string mail { get; set; }
    }
}
