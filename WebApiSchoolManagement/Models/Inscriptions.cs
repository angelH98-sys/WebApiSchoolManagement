using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.Models
{
    public class Inscriptions
    {

        public int id { get; set; }
        [Range(0, 10, ErrorMessage = "Nota general debe ser entre {1} y {2}")]
        public decimal generalgrade { get; set; } = 0;
        public string inscriptionstatus { get; set; } = "Active";
        [Range(0, 100, ErrorMessage = "Progreso debe ser entre {1} y {2}")]
        public int progress { get; set; } = 0;
        [Range(0, 10, ErrorMessage = "Promedio debe ser entre {1} y {2}")]
        public decimal avarage { get; set; } = 0;
        [Required]
        public int idStudent { get; set; }
        [Required]
        public int idCourse { get; set; }
        [Required]
        public int idTeacher { get; set; }

    }
}
