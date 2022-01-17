using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.Models
{
    public class Users
    {

        public int id { get; set; }
        public string username { get; set; }
        [Required(ErrorMessage = "Contrasenia de usuario es requerida")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Contrasenia debe contener almenos 8 caracteres, almenos 1 letra mayuscula, almenos 1 letra minuscula, almenos 1 número y algún símbolo @$!%*?&")]
        public string psswd { get; set; }
        public char usertype { get; set; }
        [Required(ErrorMessage = "Mail de usuario es requerido")]
        [EmailAddress(ErrorMessage = "Formato de mail erróneo")]
        public string mail { get; set; }
    }
}
