﻿using System.ComponentModel.DataAnnotations;

namespace WebApiSchoolManagement.Models
{
    public class Grades
    {

        public int id { get; set; }
        [Required]
        [Range(0,10, ErrorMessage = "Nota debe estar entre {1} y {2}")]
        public decimal grade { get; set; }
        public decimal gradevalue { get; set; }
        [Required]
        public int idAssignment { get; set; }
        [Required]
        public int idInscription { get; set; }
    }
}
