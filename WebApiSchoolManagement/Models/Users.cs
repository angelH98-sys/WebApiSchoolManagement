using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.Models
{
    public class Users
    {

        public int id { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string psswd { get; set; }
        [Required]
        public char usertype { get; set; }
        [Required]
        public string mail { get; set; }
    }
}
