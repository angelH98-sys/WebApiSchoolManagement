using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSchoolManagement.DTO.InscriptionDTOs;
using WebApiSchoolManagement.Models;

namespace WebApiSchoolManagement.Controllers
{
    [ApiController]
    [Route("api/inscription")]
    public class InscriptionsController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public InscriptionsController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInscription([FromBody] InscriptionCreationDTO inscriptionCreationDTO)
        {
            try
            {

                //Checking if client provied real
                var isRealStudent = await context.Students
                    .AnyAsync(studentDB => studentDB.id == inscriptionCreationDTO.idStudent);

                var isRealCourse = await context.Courses.Where(courseDB =>
                        courseDB.id == inscriptionCreationDTO.idCourse &&
                        courseDB.coursestatus == "Active")
                    .AnyAsync();

                var isRealTeacher = await context.Teachers
                    .AnyAsync(teacherDB => teacherDB.id == inscriptionCreationDTO.idTeacher);


                if (!isRealStudent | !isRealCourse | !isRealTeacher)
                {
                    return NotFound();
                }

                //Checking if doesnt exist inscription with that data
                var isAvailableInscription = !await context.Inscriptions.Where(inscriptionDB =>
                        inscriptionDB.idCourse == inscriptionCreationDTO.idCourse &&
                        inscriptionDB.idTeacher == inscriptionCreationDTO.idTeacher &&
                        inscriptionDB.idStudent == inscriptionCreationDTO.idStudent)
                    .AnyAsync();

                if (!isAvailableInscription)
                {
                    return BadRequest("Ya existe una inscripcion con esos datos");
                }

                var inscription = mapper.Map<Inscriptions>(inscriptionCreationDTO);

                context.Add(inscription);

                await context.SaveChangesAsync();

            }
            catch (Exception ex) 
            {
                return StatusCode(500, "No fue posible crear inscripcion");
            }
            return Ok();
        }
    }
}
