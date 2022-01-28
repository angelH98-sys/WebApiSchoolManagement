
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSchoolManagement.DTO.CourseDTOs;
using WebApiSchoolManagement.Models;

namespace WebApiSchoolManagement.Controllers
{
    [Route("api/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public CoursesController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {

            var courses = new List<Courses>();
            try
            {

                courses = await context.Courses.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrio un error para listar cursos");
            }

            return Ok(courses);
        }

        [HttpGet("{id:int}", Name = "GetDetailedCourse")]
        public async Task<ActionResult<CourseDetailedDTO>> GetDetailedCourse(int id)
        {
            var course = new Courses();
            try
            {
                //Instead generate a complex single query, i worked with 3 querys

                //First, get course by id
                course = await context.Courses
                .FirstOrDefaultAsync(courseDB => courseDB.id == id);

                if (course == null)
                {
                    return BadRequest();
                }
                //Then, get assignments by courseId
                course.assignments = await context.Assignments
                    .Where(assignmentsDB => assignmentsDB.idCourse == course.id)
                    .ToListAsync();

                //Then, get teacher enrolled by course
                course.teacherEnrolleds = await context.TeachersEnrolleds
                    .Include(teacherEnrolledDB => teacherEnrolledDB.teacher)
                    .ThenInclude(teacherDB => teacherDB.user)
                    .Where(teacherEnrolledDB => teacherEnrolledDB.idCourse == course.id)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                //In case of problems
                return StatusCode(500, "Ocurrio un error para obtener detalle del curso");
            }

            return Ok(mapper.Map<CourseDetailedDTO>(course));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreationDTO courseCreationDTO)
        {
            //IDK why this doesnt apply automaticly, but it works
            if (!TryValidateModel(courseCreationDTO))
            {
                return BadRequest(ModelState);
            }

            var course = new Courses();
            try
            {
                course = mapper.Map<Courses>(courseCreationDTO);

                course.coursename = course.coursename.ToUpper();

                //Checking if some course has current coursename
                var isCoursenameAvailable = !await context.Courses
                    .AnyAsync(courseDB => courseDB.coursename == course.coursename);

                if (isCoursenameAvailable == false)
                {
                    return BadRequest("Nombre de curso no disponible");
                }

                course.coursestatus = "Active";

                context.Add(course);

                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                //In case of problems
                return StatusCode(500, "Ocurrio un problema para guardar el registro");
            }


            return CreatedAtRoute("GetDetailedCourse", new { id = course.id }, mapper.Map<CourseDetailedDTO>(course));

        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchCourse(int id, [FromBody] JsonPatchDocument<CourseCreationDTO> patchDocument)
        {

            if (patchDocument == null) //Checking if patch document has info
            {
                return BadRequest();
            }

            //Getting original data
            var course = await context.Courses.FirstOrDefaultAsync(courseDB => courseDB.id == id);

            if (course == null) //Checking if client gave us real data
            {
                return NotFound();
            }

            //Executing mapper
            var coursePatchDTO = mapper.Map<CourseCreationDTO>(course);

            patchDocument.ApplyTo(coursePatchDTO, ModelState);//Applying patchdocument changes to coursePathDTO

            if (!TryValidateModel(coursePatchDTO)) //Checking if accomplish validation
            {
                return BadRequest(ModelState);
            }

            coursePatchDTO.coursename = coursePatchDTO.coursename.ToUpper();

            var isCoursenameAvailable = !await context.Courses
                    .AnyAsync(courseDB => courseDB.coursename == coursePatchDTO.coursename);
            //Checking coursename availability
            if (isCoursenameAvailable == false)
            {
                return BadRequest("Nombre de curso no disponible");
            }

            mapper.Map(coursePatchDTO, course);//Persisting patchdocument changes into course variable

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //In case of disaster
                return StatusCode(500, "Error al actualizar curso");
            }

            return NoContent();
        }

        [HttpPost("{id:int}/enrollteacher/{teacherId:int}")]
        public async Task<IActionResult> EnrollTeacherToCourse(int id, int teacherId) 
        {

            try
            {
                //Checking if client provied real ids
                var isRealCourse = await context.Courses.AnyAsync(courseDB => courseDB.id == id);

                var isRealTeacher = await context.Teachers.AnyAsync(teacherDB => teacherDB.id == teacherId);

                if (!isRealCourse | !isRealTeacher)
                {
                    return NotFound();
                }

                //Setting teacherenrolled object
                var teacherEnrolled = new TeachersEnrolleds()
                {
                    idCourse = id,
                    idTeacher = teacherId,
                    enrolledstatus = "Active"
                };

                context.Add(teacherEnrolled);

                await context.SaveChangesAsync();

            }
            catch (Exception ex) 
            {
                return StatusCode(500, "No fue posible registrar maestro en el curso");
            }
            return NoContent();
        }
    }
}
