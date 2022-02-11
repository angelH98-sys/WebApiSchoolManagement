using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.TeacherDTOs
{
    public class TeacherPatchDTO
    {
        [Required]
        public string name { get; set; }
        [Required]
        [EmailAddress]
        public string mail { get; set; }
        [Required]
        public string status { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
              ErrorMessage = "Password must have atleast 8 characters, a capital letter, a small letter, a number and a symbol @$!%*?&")]
        public string password { get; set; }

    }
}
