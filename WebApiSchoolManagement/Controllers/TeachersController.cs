using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSchoolManagement.DTO;
using WebApiSchoolManagement.Models;
using WebApiSchoolManagement.Utilities;

namespace WebApiSchoolManagement.Controllers
{
    [ApiController]
    [Route("api/teachers")]
    public class TeachersController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public TeachersController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<List<Teachers>> GetAllTeachers()
        {
            var teachers = await context.Teachers.ToListAsync();

            return teachers;
        }

        [HttpGet("{id:int}", Name = "GetTeacher")]
        public async Task<ActionResult<TeacherDTO>> GetTeacher(int id) 
        {
            if (!await context.Teachers.AnyAsync(t => t.id == id))//Checking if teacher exist by id provided
                return BadRequest("No existe un maestro con ese id");

            var teacher = await context.Teachers.FirstOrDefaultAsync(t => t.id == id);

            teacher.user = await context.Users.FirstOrDefaultAsync(u => u.id == teacher.idUser);

            return mapper.Map<TeacherDTO>(teacher);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTeacher(TeacherCreationDTO teacherCreationDTO) 
        {
            Teachers teacher = mapper.Map<Teachers>(teacherCreationDTO);
            Users user = mapper.Map<Users>(teacherCreationDTO);

            if (!ModelState.IsValid) 
            {
                return BadRequest();
            }

            //Beginning of user fields

            var usernameAvailable = true;
            do//Checking if username is available
            {
                user.username = UsernameGenerator.Genarate(teacherCreationDTO.teachername);
                usernameAvailable = !context.Users.Any(u => u.username == user.username);

            } while (usernameAvailable == false);

            user.usertype = 't';

            var mailAvailable = !context.Users.Any(u => u.mail == user.mail);
            if (mailAvailable == false)//Checking if mail address is available
            { 
            
                return BadRequest("Direccion de mail no disponible");
            }

            //At this point, user is ready to be created
            context.Add(user);
            try
            {

                await context.SaveChangesAsync();
            }
            catch (Exception e) 
            {
                return StatusCode(500, "Error al crear user");
            }

            //Beginning of teacher fields

            teacher.idUser = user.id;

            //At this point, teacher is ready to be created

            context.Add(teacher);
            try
            {

                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Error al crear teacher");
            }

            teacher.user = user;

            var teacherDTO = mapper.Map<TeacherDTO>(teacher);

            return CreatedAtRoute("GetTeacher", new { id = teacher.id }, teacherDTO);

        }
    }
}
