using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WebApiSchoolManagement.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        [Range(1, 100)]
        public int coursevalue { get; set; }
        public string status { get; set; } = "Active";
        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
