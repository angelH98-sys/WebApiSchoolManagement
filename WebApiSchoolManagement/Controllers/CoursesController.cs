
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

        public CoursesController(
            ApplicationDBContext context,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreationDTO courseCreationDTO)
        {

            var course = new Course();
            try
            {
                course = mapper.Map<Course>(courseCreationDTO);

                course.name = course.name.ToUpper();

                //Checking if some course has current coursename
                var isCoursenameAvailable = !await context.Courses
                    .AnyAsync(courseDB => courseDB.name == course.name);

                if (isCoursenameAvailable == false)
                {
                    return BadRequest("Course name is not available");
                }

                context.Add(course);

                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                //In case of problems
                return StatusCode(500, "Something goes wrong creating course");
            }

            
            return CreatedAtRoute("GetDetailedCourse", new { id = course.Id }, mapper.Map<CourseDetailedDTO>(course));

        }

        [HttpGet("get/{id:int}", Name = "GetDetailedCourse")]
        public async Task<ActionResult<CourseDetailedDTO>> GetDetailedCourse(int id)
        {
            var course = new Course();
            try
            {
                //Instead generate a complex single query, i worked with 3 querys

                //First, get course by id
                course = await context.Courses
                .FirstOrDefaultAsync(courseDB => courseDB.Id == id);

                if (course == null)
                {
                    return NotFound();
                }
                //Then, get assignments by courseId
                course.Assignments = await context.Assignments
                    .Where(assignmentsDB => assignmentsDB.CourseId == course.Id)
                    .ToListAsync();

                //Then, get teacher enrolled by course
                course.TeacherEnrolleds = await context.TeachersEnrolleds
                    .Include(teacherEnrolledDB => teacherEnrolledDB.Teacher)
                    .ThenInclude(teacherDB => teacherDB.User)
                    .Where(teacherEnrolledDB => teacherEnrolledDB.CourseId == course.Id)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                //In case of problems
                return StatusCode(500, "Something goes wrong getting course");
            }

            return Ok(mapper.Map<CourseDetailedDTO>(course));
        }

        [HttpPatch("patch/{id:int}")]
        public async Task<IActionResult> PatchCourse(int id, [FromBody] JsonPatchDocument<CoursePatchDTO> patchDocument)
        {

            if (patchDocument == null) //Checking if patch document has info
            {
                return BadRequest();
            }

            //Getting original data
            var course = await context.Courses.FirstOrDefaultAsync(courseDB => courseDB.Id == id);

            if (course == null) //Checking if client gave us real data
            {
                return NotFound();
            }

            //Executing mapper
            var coursePatchDTO = mapper.Map<CoursePatchDTO>(course);

            patchDocument.ApplyTo(coursePatchDTO, ModelState);//Applying patchdocument changes to coursePathDTO

            if (!TryValidateModel(coursePatchDTO)) //Checking if accomplish validation
            {
                return BadRequest(ModelState);
            }

            if (coursePatchDTO.name != course.name) 
            {
                
                coursePatchDTO.name = coursePatchDTO.name.ToUpper();

                var isCoursenameAvailable = !await context.Courses
                        .AnyAsync(courseDB => courseDB.name == coursePatchDTO.name);
                //Checking coursename availability
                if (isCoursenameAvailable == false)
                {
                    return BadRequest("Course name not available");
                }

            }
            mapper.Map(coursePatchDTO, course);//Persisting patchdocument changes into course variable

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //In case of disaster
                return StatusCode(500, "Something goes wrong updating course");
            }

            return NoContent();
        }

        [HttpPost("enroll/teacher/{teacherId:int}/course/{courseId:int}")]
        public async Task<IActionResult> EnrollTeacherCourse(int teacherId, int courseId) 
        {
            try
            {

                //First, we check if both parameters (teacherId and courseId) are valid (if them exist and are Active)
                var isValidTeacher = await context.Teachers.AnyAsync(teacherDB =>
                                                    teacherDB.Id == teacherId
                                                    && teacherDB.status == "Active");

                if (!isValidTeacher)
                {
                    return BadRequest("Unable to enroll an inactive or nonexistent teacher");
                }

                var isValidCourse = await context.Courses.AnyAsync(courseDB =>
                                                    courseDB.Id == courseId
                                                    && courseDB.status == "Active");

                if (!isValidCourse)
                {
                    return BadRequest("Unable to enroll an inactive or nonexistent course");
                }

                //Then, we check if already exist an enroll with both paramereters
                var isTeacherAlreadyEnroll = await context.TeachersEnrolleds.AnyAsync(enrollDB =>
                                                    enrollDB.CourseId == courseId
                                                    && enrollDB.TeacherId == teacherId);

                if (isTeacherAlreadyEnroll)
                {
                    return BadRequest("Teacher already enroll to specified course");
                }


                var teacherEnroll = new TeachersEnrolled()
                {
                    TeacherId = teacherId,
                    CourseId = courseId
                };

                context.Add(teacherEnroll);

                await context.SaveChangesAsync();

            }
            catch (Exception ex) 
            {
                return StatusCode(500, "Something goes wrong on teacher enrollment");
            }
            return Ok();
        }
    }
}
