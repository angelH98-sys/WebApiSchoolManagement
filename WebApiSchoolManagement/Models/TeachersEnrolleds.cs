namespace WebApiSchoolManagement.Models
{
    public class TeachersEnrolleds
    {

        public int id { get; set; }
        public string enrolledstatus { get; set; }
        public int idTeacher { get; set; }
        public int idCourse { get; set; }
    }
}
