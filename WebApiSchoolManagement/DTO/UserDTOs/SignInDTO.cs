using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.DTO
{
    public class SignInDTO
    {
        [Required]
        public string username { get; set; }
        [Required]
        [EmailAddress]
        public string mail { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string role { get; set; }
    }
}
