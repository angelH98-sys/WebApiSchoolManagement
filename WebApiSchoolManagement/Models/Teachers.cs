using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiSchoolManagement.Models
{
    public class Teachers
    {

        public int id { get; set; }
        [Required]
        public string teachername { get; set; }
        [Required]
        public int idUser { get; set; }

        [NotMapped]
        public Users user { get; set; }
    }
}
