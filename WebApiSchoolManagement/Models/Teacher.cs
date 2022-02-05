using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiSchoolManagement.Models
{
    public class Teacher
    {

        public int Id { get; set; }
        [Required]
        public string name { get; set; }
        public string status { get; set; } = "Active";
        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
