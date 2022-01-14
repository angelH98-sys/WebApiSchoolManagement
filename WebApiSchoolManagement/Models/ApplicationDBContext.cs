using Microsoft.EntityFrameworkCore;

namespace WebApiSchoolManagement.Models
{
    public class ApplicationDBContext : DbContext 
    {

        public ApplicationDBContext(DbContextOptions options): base(options) {}

        public DbSet<Users> Users { get; set; }
        public DbSet<Teachers> Teachers { get; set; }
        public DbSet<Students> Students { get; set; }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<Assignments> Assignments { get; set; }
        public DbSet<TeachersEnrolleds> TeachersEnrolleds { get; set; }
        public DbSet<Inscriptions> Inscriptions { get; set; }
        public DbSet<Grades> Grades { get; set; }
    }
}
