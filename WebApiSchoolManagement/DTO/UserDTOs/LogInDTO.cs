using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO.UserDTOs
{
    public class LogInDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
