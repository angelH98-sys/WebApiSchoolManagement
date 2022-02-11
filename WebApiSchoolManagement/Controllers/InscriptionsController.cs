using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSchoolManagement.DTO.InscriptionDTOs;
using WebApiSchoolManagement.Models;

namespace WebApiSchoolManagement.Controllers
{
    [ApiController]
    [Route("api/inscription")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]

    public class InscriptionsController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public InscriptionsController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateInscription([FromBody] InscriptionCreationDTO inscriptionCreationDTO)
        {
            try
            {

                //Checking if client provied real
                var isValidStudent = await context.Students.AnyAsync(studentDB =>
                                            studentDB.Id == inscriptionCreationDTO.StudentId
                                            && studentDB.status == "Active");

                if (!isValidStudent)
                {
                    return BadRequest("Unable to enroll an inactive or nonexistent student");
                }

                var isValidCourse = await context.Courses.AnyAsync(courseDB =>
                                            courseDB.Id == inscriptionCreationDTO.CourseId
                                            && courseDB.status == "Active");

                if (!isValidCourse)
                {
                    return BadRequest("Unable to enroll an inactive or nonexistent course");
                }

                var isValidTeacher = await context.Teachers.AnyAsync(teacherDB =>
                                             teacherDB.Id == inscriptionCreationDTO.TeacherId
                                             && teacherDB.status == "Active");

                if (!isValidCourse)
                {
                    return BadRequest("Unable to enroll an inactive or nonexistent teacher");
                }

                //Checking if teacher is enrolled to course
                var irTeacherEnroll = await context.TeachersEnrolleds.AnyAsync(enrollDB =>
                                            enrollDB.TeacherId == inscriptionCreationDTO.TeacherId
                                            && enrollDB.CourseId == inscriptionCreationDTO.CourseId
                                            && enrollDB.status == "Active");

                if (!irTeacherEnroll) 
                {
                    return BadRequest("Unable to enroll teacher with specified course");
                }


                //Checking if student has the same courseyear as course
                var studentCourseYear = await context.Students
                    .Where(studentDB => studentDB.Id == inscriptionCreationDTO.StudentId)
                    .Select(std => new Student() { courseyear = std.courseyear })
                    .FirstOrDefaultAsync();

                var courseCourseYear = await context.Courses
                    .Where(courseDB => courseDB.Id == inscriptionCreationDTO.CourseId)
                    .Select(crs => new Course() { courseyear = crs.courseyear })
                    .FirstOrDefaultAsync();

                if (studentCourseYear.courseyear != courseCourseYear.courseyear) 
                {
                    return BadRequest("Student unable to enroll to course");
                }


                //Checking if doesnt exist inscription with that data
                var isAvailableInscription = await context.Inscriptions.Where(inscriptionDB =>
                        inscriptionDB.CourseId == inscriptionCreationDTO.CourseId &&
                        inscriptionDB.TeacherId == inscriptionCreationDTO.TeacherId &&
                        inscriptionDB.StudentId == inscriptionCreationDTO.StudentId)
                    .AnyAsync();

                if (isAvailableInscription)
                {
                    return BadRequest("Inscription already exist with data provided");
                }

                var inscription = mapper.Map<Inscription>(inscriptionCreationDTO);

                context.Add(inscription);

                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something goes wrong on inscription creation");
            }
            return Ok();
        }

        [HttpGet("get/course/{courseId:int}")]
        public async Task<ActionResult<List<InscriptionDTO>>> GetInscriptionByCourse(int courseId) 
        {
            try
            {
                //First, we check if client provied us a real courseid
                var isRealCourse = await context.Courses.AnyAsync(courseDB => courseDB.Id == courseId);

                if (!isRealCourse)
                {
                    return NotFound();
                }

                var inscriptionList = await context.Inscriptions
                                                .Include(inscriptionDB => inscriptionDB.Student)
                                                .ThenInclude(studentDB => studentDB.User)
                                                .Include(inscriptionDB => inscriptionDB.Teacher)
                                                .ThenInclude(teacherDB => teacherDB.User)
                                                .Where(inscriptionDB => inscriptionDB.CourseId == courseId)
                                                .ToListAsync();

                if (inscriptionList == null)
                {
                    return NotFound();
                }

                var inscriptionDTOList = new List<InscriptionDTO>();
                foreach(Inscription inscription in inscriptionList)
                    inscriptionDTOList.Add(mapper.Map<InscriptionDTO>(inscription));

                return inscriptionDTOList;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something goes wrong getting inscriptions");
            }
        }
    }
}
