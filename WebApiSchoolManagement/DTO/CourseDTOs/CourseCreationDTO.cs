using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.CourseDTOs
{
    public class CourseCreationDTO
    {
        [Required]
        public string coursename { get; set; }
        [Required]
        [Range(1, 9, ErrorMessage = "Anio de curso debe ser entre {1} y {2}")]
        public int courseyear { get; set; }
    }
}
