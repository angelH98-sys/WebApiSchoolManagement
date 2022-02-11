using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.CourseDTOs
{
    public class CourseCreationDTO
    {
        [Required]
        public string name { get; set; }
        [Required]
        [Range(1, 9)]
        public int courseyear { get; set; }
    }
}
