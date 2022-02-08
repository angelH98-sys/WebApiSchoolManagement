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

        public AssignmentsController(
            ApplicationDBContext context,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAssignment(int courseId, [FromBody] AssignmentCreationDTO assignmentCreationDTO)
        {
            try
            {

                //First, lets check if client provied us a real course id
                var existCourse = await context.Courses
                    .AnyAsync(coursesDB => coursesDB.Id == courseId);

                if (existCourse == false)
                {
                    return NotFound();
                }

                //Setting assignment objetc
                var assignment = new Assignment();
                assignment = mapper.Map<Assignment>(assignmentCreationDTO);
                assignment.name = assignment.name.ToUpper();
                assignment.CourseId = courseId;

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
                return StatusCode(500, "Something goes wrong creating assignment");
            }
            return Ok();
        }

        private bool isAssignmentnameAvailable(Assignment assignment)
        {
            //Validating if assignment name is available
            var availableAssignmentName = !context.Assignments.Where(assignmentDB =>
                        assignmentDB.CourseId == assignment.CourseId &&
                        assignmentDB.status == "Active" &&
                        assignmentDB.Id != assignment.Id &&
                        assignmentDB.name == assignment.name)
                .Any();

            if (availableAssignmentName == false)
            {
                ModelState.AddModelError("name", "Assignment name not available");
                return false;
            }
            return true;
        }


        private bool isEnoughAvailableValue(Assignment assignment)
        {
            //Getting current value availability fron a course

            //Firs, getting every coursevalue from assignments by courseid
            var assignments = context.Assignments.Where(assignmentDB =>
                        assignmentDB.CourseId == assignment.CourseId &&
                        assignmentDB.status == "Active" &&
                        assignmentDB.Id != assignment.Id)
                .Select(assgn => new Assignment { coursevalue = assgn.coursevalue })
                .ToList();

            var availableValue = 100;

            foreach (Assignment assgn in assignments)
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

        [HttpPatch("patch/{id:int}")]
        public async Task<IActionResult> PatchAssignment(int courseId, int id, JsonPatchDocument<AssignmentPatchDTO> patchDocument)
        {
            try
            {

                var assignment = await context.Assignments.FirstOrDefaultAsync(assignmentDB => 
                                assignmentDB.CourseId == courseId 
                                && assignmentDB.Id == id);

                if (assignment == null)
                {
                    return NotFound();
                }

                var assignmentPatchDTO = mapper.Map<AssignmentPatchDTO>(assignment);

                patchDocument.ApplyTo(assignmentPatchDTO, ModelState);

                if (!TryValidateModel(assignmentPatchDTO))
                {
                    return BadRequest(ModelState);
                }

                assignmentPatchDTO.name = assignmentPatchDTO.name.ToUpper();

                mapper.Map(assignmentPatchDTO, assignment);

                if (!isAssignmentnameAvailable(assignment) | !isEnoughAvailableValue(assignment))
                {
                    return BadRequest(ModelState);
                }

                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something goes wrong updating assignment");
            }
            return NoContent();
        }
       
    }
}
