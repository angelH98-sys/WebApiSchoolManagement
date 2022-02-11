namespace WebApiSchoolManagement.DTO.CourseDTOs
{
    public class CourseDTO
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string status { get; set; } = "Active";
    }
}
