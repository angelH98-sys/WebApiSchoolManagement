using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApiSchoolManagement.Models
{
    public class ApplicationDBContext : IdentityDbContext 
    {

        public ApplicationDBContext(DbContextOptions options): base(options) {}

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<TeachersEnrolled> TeachersEnrolleds { get; set; }
        public DbSet<Inscription> Inscriptions { get; set; }
        public DbSet<Grade> Grades { get; set; }
    }
}
