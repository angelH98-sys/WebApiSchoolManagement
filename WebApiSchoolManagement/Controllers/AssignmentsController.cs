using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSchoolManagement.DTO.AssignmentDTOs;
using WebApiSchoolManagement.Models;

namespace WebApiSchoolManagement.Controllers
{
    [ApiController]
    [Route("api/courses/{courseId:int}/assignments")]
    public class AssignmentsController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public AssignmentsController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAssignmentsByCourse(int courseId)
        {

            var assignmentsByCourseDTO = new List<AssignmentByCourseDTO>();
            try
            {

                //First, lets check if client provied us a real course id
                var existCourse = await context.Courses
                    .AnyAsync(coursesDB => coursesDB.id == courseId);

                if (existCourse == false)
                {
                    return NotFound();
                }

                //Getting all assignments by course id
                var assignments = await context.Assignments
                    .Where(assignmentDB => assignmentDB.idCourse == courseId).ToListAsync();
                //Then, mapping to assignmentsbycourseDTO
                assignmentsByCourseDTO = mapper.Map<List<AssignmentByCourseDTO>>(assignments);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "No fue posible obtener las asignaciones");
            }

            return Ok(assignmentsByCourseDTO);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssignment(int courseId, [FromBody] AssignmentCreationDTO assignmentCreationDTO)
        {
            try
            {

                //First, lets check if client provied us a real course id
                var existCourse = await context.Courses
                    .AnyAsync(coursesDB => coursesDB.id == courseId);

                if (existCourse == false)
                {
                    return NotFound();
                }

                //Setting assignment objetc
                var assignment = mapper.Map<Assignments>(assignmentCreationDTO);
                assignment.assignmentname = assignment.assignmentname.ToUpper();
                assignment.idCourse = courseId;
                assignment.assignmentstatus = "Active";

                //Checking if assignment name is available and if course value is enough
                if (!isAssignmentnameAvailable(assignment) | !isEnoughAvailableValue(assignment))
                {
                    return BadRequest(ModelState);
                }

                context.Add(assignment);

                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrio un problema al crear asignacion");
            }
            return Ok();
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchAssignment(int courseId, int id, JsonPatchDocument<AssignmentCreationDTO> patchDocument) 
        {
            try
            {

                var assignment = await context.Assignments
                    .Where(assignmentDB => assignmentDB.idCourse == courseId)
                    .FirstOrDefaultAsync(assignmentDB => assignmentDB.id == id);

                if (assignment == null)
                {
                    return NotFound();
                }

                var assignmentCreationDTO = mapper.Map<AssignmentCreationDTO>(assignment);

                patchDocument.ApplyTo(assignmentCreationDTO, ModelState);

                if (!TryValidateModel(assignmentCreationDTO))
                {
                    return BadRequest(ModelState);
                }

                assignmentCreationDTO.assignmentname = assignmentCreationDTO.assignmentname.ToUpper();

                mapper.Map(assignmentCreationDTO, assignment);

                if (!isAssignmentnameAvailable(assignment) | !isEnoughAvailableValue(assignment))
                {
                    return BadRequest(ModelState);
                }

                await context.SaveChangesAsync();

            }
            catch (Exception ex) 
            {
                return StatusCode(500, "No fue posible actualizar asignacion");
            }
            return NoContent();
        }

        private bool isAssignmentnameAvailable(Assignments assignment) 
        {
            //Validating if assignment name is available
            var availableAssignmentName = !context.Assignments.Where(assignmentDB => 
                        assignmentDB.idCourse == assignment.idCourse &&
                        assignmentDB.assignmentstatus == "Active" &&
                        assignmentDB.id != assignment.id &&
                        assignmentDB.assignmentname == assignment.assignmentname)
                .Any();

            if (availableAssignmentName == false)
            {
                ModelState.AddModelError("assignmentname", "Assignment name not available");
                return false;
            }
            return true;
        }


        private bool isEnoughAvailableValue(Assignments assignment) 
        {
            //Getting current value availability fron a course

            //Firs, getting every coursevalue from assignments by courseid
            var assignments = context.Assignments.Where(assignmentDB => 
                        assignmentDB.idCourse == assignment.idCourse &&
                        assignmentDB.assignmentstatus == "Active" &&
                        assignmentDB.id != assignment.id)
                .Select(assgn => new Assignments { coursevalue = assgn.coursevalue })
                .ToList();

            var availableValue = 100;

            foreach (Assignments assgn in assignments) 
            {
                availableValue -= assgn.coursevalue;
            }

            if (availableValue < assignment.coursevalue) 
            {
                ModelState.AddModelError("coursevalue", "Assignment value not available");
                return false;
            }

            return true;
        }
    }
}
