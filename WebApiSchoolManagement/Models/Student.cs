using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiSchoolManagement.Models
{
    public class Student
    {

        public int Id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        [Range(1, 9)]
        public int courseyear { get; set; }
        public string status { get; set; } = "Active";
        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
