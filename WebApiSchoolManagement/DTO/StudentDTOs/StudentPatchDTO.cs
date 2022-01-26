using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.StudentDTOs
{
    public class StudentPatchDTO
    {
        [Required]
        public string studentname { get; set; }
        [Required]
        [Range(1, 9, ErrorMessage = "Anio de curso debe ser entre {1} y {2}")]
        public int courseyear { get; set; }
        [Required(ErrorMessage = "Mail de usuario es requerido")]
        [EmailAddress(ErrorMessage = "Formato de mail erróneo")]
        public string mail { get; set; }
    }
}
