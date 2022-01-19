using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WebApiSchoolManagement.DTO.TeacherDTOs;
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
                return StatusCode(500, "Error al crear maestro");
            }

            teacher.user = user;

            var teacherDTO = mapper.Map<TeacherDTO>(teacher);

            return CreatedAtRoute("GetTeacher", new { id = teacher.id }, teacherDTO);

        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchTeacher(int id, JsonPatchDocument<TeacherPatchDTO> patchDocument)
        {
            if (patchDocument == null)//Checking if patch document is missing 
            {
                return BadRequest();
            }

            var teacher = await context.Teachers.FirstOrDefaultAsync(teacherDB => teacherDB.id == id);

            if (teacher == null) // Checking if client provides a real teacher id
            {
                return NotFound();
            }

            var user = await context.Users.FirstOrDefaultAsync(userDB => userDB.id == teacher.idUser);

            if (user == null) //Just in case, if user doesnt exist
            {
                return StatusCode(500, "Error al cargar informacion de usuario");
            }

            teacher.user = user;

            var teacherPatchDTO = mapper.Map<TeacherPatchDTO>(teacher);

            patchDocument.ApplyTo(teacherPatchDTO, ModelState);//Applying patch document modified data to DTO

            if (!TryValidateModel(teacherPatchDTO))
            {
                return BadRequest(ModelState);
            }

            if (teacherPatchDTO.mail != teacher.user.mail) //Only if mail is changed 
            {

                var availableMail = !await context.Users.AnyAsync(userDB => userDB.mail == teacherPatchDTO.mail);

                if (availableMail == false)
                {
                    return BadRequest("Direccion de mail no disponible");
                }
            }

            mapper.Map(teacherPatchDTO, teacher);
            mapper.Map(teacherPatchDTO, user);

            try
            {

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al actualizar maestro");
            }

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PatchTeacherPassword(int id,[FromBody] string psswd) 
        {
            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");

            if (!regex.IsMatch(psswd)) 
            {

                return BadRequest("Contrasenia debe contener almenos 8 caracteres, almenos 1 letra mayuscula, almenos 1 letra minuscula, almenos 1 número y algún símbolo @$!%*?&");
            }

            var teacher = await context.Teachers.FirstOrDefaultAsync(teacherDB => teacherDB.id == id);

            if (teacher == null) 
            {
                return NotFound();
            }

            teacher.user = await context.Users.FirstOrDefaultAsync(userDB => userDB.id == teacher.idUser);

            if (teacher.user == null) 
            {
                return StatusCode(500, "Error al cargar informacion de usuarios");
            }

            teacher.user.psswd = psswd;

            try
            {

                await context.SaveChangesAsync();
            }
            catch (Exception ex) 
            {

                return StatusCode(500, "Error al actualizar contrasenia de maestro");
            }

            return NoContent();
        }
    }
}
