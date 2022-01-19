using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.TeacherDTOs
{
    public class TeacherPatchDTO
    {
        [Required(ErrorMessage = "Nombre es requerido")]
        public string teachername { get; set; }
        [Required(ErrorMessage = "Mail de usuario es requerido")]
        [EmailAddress(ErrorMessage = "Formato de mail erróneo")]
        public string mail { get; set; }
    }
}
