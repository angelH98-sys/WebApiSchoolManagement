using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.TeacherDTOs
{
    public class TeacherPatchDTO
    {
        public string name { get; set; }
        [EmailAddress]
        public string mail { get; set; }
        public string status { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
              ErrorMessage = "Password must have atleast 8 characters, a capital letter, a small letter, a number and a symbol @$!%*?&")]
        public string password { get; set; }

    }
}
