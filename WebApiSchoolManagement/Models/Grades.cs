namespace WebApiSchoolManagement.Models
{
    public class Grades
    {

        public int id { get; set; }
        public decimal grade { get; set; }
        public decimal gradevalue { get; set; }
        public int idAssignment { get; set; }
        public int idInscription { get; set; }
    }
}
