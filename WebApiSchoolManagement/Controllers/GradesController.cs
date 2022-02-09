using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSchoolManagement.DTO.GradeDTOs;
using WebApiSchoolManagement.Models;

namespace WebApiSchoolManagement.Controllers
{
    [ApiController]
    [Route("api/grades")]
    public class GradesController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public GradesController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateGrade([FromBody] GradeCreationDTO gradeCreationDTO) 
        {

            //Checking if grade already exist
            var gradeAlreadyExist = await context.Grades.Where(gradeDB =>
                gradeDB.AssignmentId == gradeCreationDTO.AssignmentId &&
                gradeDB.InscriptionId == gradeCreationDTO.InscriptionId)
                .AnyAsync();

            if (gradeAlreadyExist)
            {
                return BadRequest("Grade already exist for provided data");
            }

            //Getting assignment and inscription to the process

            var inscription = await context.Inscriptions
                .Where(inscriptionDB => inscriptionDB.Id == gradeCreationDTO.InscriptionId)
                .FirstOrDefaultAsync();

            if (inscription == null)
            {
                return NotFound();
            }

            if (inscription.status != "Active") 
            {
                return BadRequest("Unable to create a grade of an inactive inscription");
            }

            //For the assignment, i just need course value to calculate the percentage in grade object
            //grade.gradevalue

            var assignment = await context.Assignments
                .Where(assignmentDB => assignmentDB.Id == gradeCreationDTO.AssignmentId 
                                        && assignmentDB.CourseId == inscription.CourseId)
                .Select(assgn => new Assignment { coursevalue = assgn.coursevalue, status = assgn.status })
                .FirstOrDefaultAsync();

            if (assignment == null) 
            {
                return NotFound();
            }

            if (assignment.status != "Active") 
            {
                return BadRequest("Unable to create a grade of an inactive assignment");
            }
            

            //Create a grade object
            var grade = mapper.Map<Grade>(gradeCreationDTO);

            grade.gradevalue = grade.grade * assignment.coursevalue / 10;

            inscription.generalgrade += grade.gradevalue;

            inscription.progress += assignment.coursevalue;

            inscription.avarage = updateInscriptionAvarage(inscription.Id, grade.grade);

            context.Add(grade);

            await context.SaveChangesAsync();

            return Ok();
        }

        private decimal updateInscriptionAvarage(int inscriptionId, decimal grade)
        {
            var grades = context.Grades
                .Where(gradeDB => gradeDB.InscriptionId == inscriptionId)
                .Select(grd => new Grade() { grade = grd.grade })
                .ToList();

            decimal avarage = grade;
            foreach (Grade grd in grades) 
            {
                avarage += grd.grade;   
            }

            avarage = avarage / (grades.Count() + 1);

            return avarage;
        }
    }
}
