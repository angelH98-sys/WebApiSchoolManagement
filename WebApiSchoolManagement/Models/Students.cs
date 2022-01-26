using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiSchoolManagement.Models
{
    public class Students
    {

        public int id { get; set; }
        [Required]
        public string studentname { get; set; }
        [Required]
        [Range(1, 9, ErrorMessage = "Anio de curso debe ser entre {1} y {2}")]
        public int courseyear { get; set; }
        [Required]
        public int idUser { get; set; }
        [ForeignKey("idUser")]
        public Users user { get; set; }
    }
}
