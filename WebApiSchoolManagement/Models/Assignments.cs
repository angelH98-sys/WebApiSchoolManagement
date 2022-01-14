namespace WebApiSchoolManagement.Models
{
    public class Assignments
    {
        public int id { get; set; }
        public string assignmentname { get; set; }
        public int coursevalue { get; set; }
        public string assignmentstatus { get; set; }
        public int idCourse { get; set; }
        public Courses course { get; set; }
    }
}
