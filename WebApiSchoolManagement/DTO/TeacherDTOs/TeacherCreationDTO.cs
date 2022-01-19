using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.TeacherDTOs
{
    public class TeacherCreationDTO
    {
        [Required(ErrorMessage = "Nombre es requerido")]
        public string teachername { get; set; }
        [Required(ErrorMessage = "Contrasenia es requerida")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Contrasenia debe contener almenos 8 caracteres, almenos 1 letra mayuscula, almenos 1 letra minuscula, almenos 1 número y algún símbolo @$!%*?&")]
        public string psswd { get; set; }
        [Required(ErrorMessage = "Mail de usuario es requerido")]
        [EmailAddress(ErrorMessage = "Formato de mail erróneo")]
        public string mail { get; set; }
    }
}
