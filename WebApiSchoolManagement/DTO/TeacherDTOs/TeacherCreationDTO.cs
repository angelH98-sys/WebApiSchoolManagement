using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.TeacherDTOs
{
    public class TeacherCreationDTO
    {
        [Required]
        public string teachername { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
              ErrorMessage = "Password must have atleast 8 characters, a capital letter, a small letter, a number and a symbol @$!%*?&")]  
            //ErrorMessage = "Contrasenia debe contener almenos 8 caracteres, almenos 1 letra mayuscula, almenos 1 letra minuscula, almenos 1 número y algún símbolo ")]
        public string psswd { get; set; }
        [Required]
        [EmailAddress]
        public string mail { get; set; }
    }
}
