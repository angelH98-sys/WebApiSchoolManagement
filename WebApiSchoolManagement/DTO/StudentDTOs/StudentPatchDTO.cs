using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.StudentDTOs
{
    public class StudentPatchDTO
    {
        [Required]
        public string name { get; set; }
        [Required]
        [Range(1, 9)]
        public int courseyear { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
              ErrorMessage = "Password must have atleast 8 characters, a capital letter, a small letter, a number and a symbol @$!%*?&")]
        public string password { get; set; }
        [Required]
        [EmailAddress]
        public string mail { get; set; }
    }
}
