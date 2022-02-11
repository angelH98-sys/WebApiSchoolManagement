using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using WebApiSchoolManagement.DTO.GradeDTOs;
using WebApiSchoolManagement.Models;

namespace WebApiSchoolManagement.Controllers
{
    [ApiController]
    [Route("api/grades")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GradesController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private IdentityUser userLogged = new IdentityUser();
        private string role = ""
;
        public GradesController(
            ApplicationDBContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateGrade([FromBody] GradeCreationDTO gradeCreationDTO) 
        {
            try
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

                //Checking if user logged were enrolle with the inscription
                if (role == "Teacher") 
                {
                    if (inscription.Teacher.UserId != userLogged.Id) 
                    {
                        return Unauthorized();
                    }
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
            catch (Exception ex) 
            {
                return StatusCode(500, "Something goes wrong creating a grade");
            }

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
        private async Task<ActionResult> GetUserLogged()
        {
            Request.Headers.TryGetValue("Authorization", out var headerValue);

            var jwt = headerValue[0].Split(" ")[1];

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwt);
            var tokenS = jsonToken as JwtSecurityToken;

            var mail = tokenS.Claims.First(claim => claim.Type == "mail").Value;

            userLogged = await userManager.FindByEmailAsync(mail);

            if (tokenS.Claims.Any(claim => claim.Type == "Admin"))
                role = "Admin";
            
            if (tokenS.Claims.Any(claim => claim.Type == "Teacher"))
                role = "Teacher";

            return Ok();
        }

    }
}
