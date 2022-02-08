using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.CourseDTOs
{
    public class CoursePatchDTO : CourseCreationDTO
    {
        [Required]
        public string status { get; set; }
    }
}
