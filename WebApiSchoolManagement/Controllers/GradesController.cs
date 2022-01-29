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

        [HttpPost]
        public async Task<IActionResult> CreateGrade([FromBody] GradeCreationDTO gradeCreationDTO) 
        {
            //Getting assignment and inscription to the process

            //For the assignment, i just need course value to calculate the percentage in grade object
            //grade.gradevalue
            var assignment = await context.Assignments
                .Where(assignmentDB => assignmentDB.id == gradeCreationDTO.idAssignment)
                .Select(assgn => new Assignments { coursevalue = assgn.coursevalue })
                .FirstOrDefaultAsync();

            var inscription = await context.Inscriptions
                .Where(inscriptionDB => inscriptionDB.id == gradeCreationDTO.idInscription &&
                inscriptionDB.inscriptionstatus == "Active")
                .FirstOrDefaultAsync();

            if (assignment == null | inscription == null)
            {
                return NotFound();
            }

            //Checking if grade already exist
            var gradeAlreadyExist = await context.Grades.Where(gradeDB =>
                gradeDB.idAssignment == gradeCreationDTO.idAssignment &&
                gradeDB.idInscription == gradeCreationDTO.idInscription)
                .AnyAsync();

            if (gradeAlreadyExist) 
            {
                return BadRequest("Nota ya existente");
            }

            //Create a grade object
            var grade = mapper.Map<Grades>(gradeCreationDTO);

            grade.gradevalue = grade.grade * assignment.coursevalue / 10;

            inscription.generalgrade += grade.gradevalue;

            inscription.progress += assignment.coursevalue;

            inscription.avarage = updateInscriptionAvarage(inscription.id, grade.grade);

            context.Add(grade);

            await context.SaveChangesAsync();

            return Ok();
        }

        private decimal updateInscriptionAvarage(int inscriptionId, decimal grade)
        {
            var grades = context.Grades
                .Where(gradeDB => gradeDB.idInscription == inscriptionId)
                .Select(grd => new Grades() { grade = grd.grade })
                .ToList();

            decimal avarage = grade;
            foreach (Grades grd in grades) 
            {
                avarage += grd.grade;   
            }

            avarage = avarage / (grades.Count() + 1);

            return avarage;
        }
    }
}
