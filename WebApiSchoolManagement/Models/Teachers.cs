using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiSchoolManagement.Models
{
    public class Teachers
    {

        public int id { get; set; }
        [Required(ErrorMessage = "Nombre es requerido")]
        public string teachername { get; set; }
        [Required(ErrorMessage = "ID de usuario es requerido")]
        public int idUser { get; set; }
        [NotMapped]
        public Users user { get; set; }
    }
}
