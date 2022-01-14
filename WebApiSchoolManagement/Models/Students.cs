namespace WebApiSchoolManagement.Models
{
    public class Students
    {

        public int id { get; set; }
        public string studentname { get; set; }
        public int courseyear { get; set; }
        public int idUser { get; set; }
        public Users user { get; set; }
    }
}
