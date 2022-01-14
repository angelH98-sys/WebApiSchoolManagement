using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSchoolManagement.Models;

namespace WebApiSchoolManagement.Controllers
{
    [ApiController]
    [Route("api/teachers")]
    public class TeachersController : ControllerBase
    {
        private readonly ApplicationDBContext context;

        public TeachersController(ApplicationDBContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<List<Teachers>> GetAllTeachers() 
        {
            var teachers = await context.Teachers.ToListAsync();

            return teachers;
        }
    }
}
