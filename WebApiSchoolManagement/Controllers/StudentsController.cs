using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WebApiSchoolManagement.DTO.StudentDTOs;
using WebApiSchoolManagement.Models;
using WebApiSchoolManagement.Utilities;

namespace WebApiSchoolManagement.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public StudentsController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var studentList = new List<Students>();
            try
            {
                //Getting students and users
                studentList = await context.Students
                    .Include(studentDB => studentDB.user)
                    .ToListAsync();

            } catch (Exception ex)
            {//Just if last query fails, generic error
                return StatusCode(500, "Error al cargar estudiantes");
            }

            var studentDTOList = mapper.Map<List<StudentDTO>>(studentList);//Executing mapping

            return Ok(studentDTOList);
        }

        [HttpGet("{id:int}", Name = "GetStudent")]
        public async Task<IActionResult> GetStudent(int id)
        {
            //Variable declares
            var student = new Students();
            var studentDTO = new StudentDTO();
            try
            {
                //Getting student and user info
                student = await context.Students
                    .Include(studentDB => studentDB.user)
                    .FirstOrDefaultAsync(studentDB => studentDB.id == id);

                studentDTO = mapper.Map<StudentDTO>(student);
            }
            catch (Exception ex)
            {
                //Just in case, to not retorn server error
                return StatusCode(500, "Error al cargar estudiante");
            }

            return Ok(studentDTO);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent(StudentCreationDTO studentCreationDTO)
        {

            //2 Tables, 2 Maps
            var student = mapper.Map<Students>(studentCreationDTO);
            var user = mapper.Map<Users>(studentCreationDTO);

            var usernameIsAvailable = true;

            do
            {
                //Checking if username generated is available
                user.username = UsernameGenerator.Genarate(student.studentname);
                usernameIsAvailable = !context.Users.Any(userDB => userDB.username == user.username);
            } while (usernameIsAvailable == false);

            user.usertype = 's';//Assigning usertype

            var mailAvailable = !context.Users.Any(u => u.mail == user.mail);
            if (mailAvailable == false)//Checking if mail address is available
            {

                return BadRequest("Direccion de mail no disponible");
            }

            //Trying to add user
            context.Add(user);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //If something goes wrong
                return StatusCode(500, "Error al crear usuario");
            }

            student.idUser = user.id;//Adding user id to student object


            //Trying to add student
            context.Add(student);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //If something goes wrong
                return StatusCode(500, "Error al crear estudiante");
            }

            student.user = user;//Adding user object to student property for mapper

            return CreatedAtRoute("GetStudent", new { id = student.id }, mapper.Map<StudentDTO>(student));
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchStudent(int id, JsonPatchDocument<StudentPatchDTO> patchDocument) 
        {

            if (patchDocument == null) //Checking if doc isnt null
            {
                return BadRequest();
            }

            //Getting original data from student
            var student = await context.Students.FirstOrDefaultAsync(studentDB => studentDB.id == id);

            if (student == null) // Checking if client provides a real student id
            {
                return NotFound();
            }

            //Getting original data from user
            var user = await context.Users.FirstOrDefaultAsync(userDB => userDB.id == student.idUser);

            if (user == null) //Just in case, if user doesnt exist
            {
                return StatusCode(500, "Error al cargar informacion de usuario");
            }

            student.user = user;

            var studentPatchDTO = mapper.Map<StudentPatchDTO>(student);

            patchDocument.ApplyTo(studentPatchDTO, ModelState);//Applying patch document modified data to DTO

            if (!TryValidateModel(studentPatchDTO))
            {
                return BadRequest(ModelState);
            }

            if (studentPatchDTO.mail != student.user.mail) //Only if mail is changed 
            {

                var availableMail = !await context.Users.AnyAsync(userDB => userDB.mail == studentPatchDTO.mail);

                if (availableMail == false)
                {
                    return BadRequest("Direccion de mail no disponible");
                }
            }

            mapper.Map(studentPatchDTO, student);
            mapper.Map(studentPatchDTO, user);

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
        public async Task<IActionResult> PatchStudentPassword(int id, [FromBody] string psswd)
        {
            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");

            if (!regex.IsMatch(psswd)) //Checking password format
            {

                return BadRequest("Contrasenia debe contener almenos 8 caracteres, almenos 1 letra mayuscula, almenos 1 letra minuscula, almenos 1 número y algún símbolo @$!%*?&");
            }

            //Getting initial data
            var student = await context.Students
                .Include(studentDB => studentDB.user)
                .FirstOrDefaultAsync(studentDB => studentDB.id == id);

            if (student == null) //Checking if student exist
            {
                return NotFound();
            }

            student.user.psswd = psswd;

            try
            {

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Error al actualizar contrasenia de estudiante");
            }

            return NoContent();
        }
    }
}
