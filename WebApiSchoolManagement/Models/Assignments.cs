using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WebApiSchoolManagement.Models
{
    public class Assignments
    {
        public int id { get; set; }
        [Required]
        public string assignmentname { get; set; }
        [Required]
        [Range(1, 100,
            ErrorMessage = "Valor del curso debe ser entre {1} y {2}")]
        public int coursevalue { get; set; }
        public string assignmentstatus { get; set; }
        [Required]
        public int idCourse { get; set; }
        [ForeignKey("idCourse")]
        public Courses course { get; set; }
    }
}
