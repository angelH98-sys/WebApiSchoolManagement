using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using WebApiSchoolManagement.DTO.StudentDTOs;
using WebApiSchoolManagement.Models;
using WebApiSchoolManagement.Utilities;

namespace WebApiSchoolManagement.Controllers
{
    [ApiController]
    [Route("api/students")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly HashService hashService;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly string role = "Student";

        public StudentsController(
            ApplicationDBContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            HashService hashService,
            RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.hashService = hashService;
            this.roleManager = roleManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStudent([FromBody] StudentCreationDTO studentCreationDTO)
        {
            var student = mapper.Map<Student>(studentCreationDTO);
            try
            {


                var isMailAvailable = await userManager.FindByEmailAsync(studentCreationDTO.mail);
                //Checking if user mail is available
                if (isMailAvailable != null)
                {
                    return BadRequest("Mail address not available");
                }

                student.User = await CreateUser(studentCreationDTO.mail, studentCreationDTO.password);

                if (student.User == null)
                    //If user creation is successfull
                {
                    return StatusCode(500, "Something goes wrong on user creation");
                }

                student.UserId = student.User.Id;

                context.Add(student);
                
                await context.SaveChangesAsync();
                
            }
            catch (Exception ex) 
            {
                return StatusCode(500, "Something goes wrong on student creation");
            }


            return CreatedAtRoute("GetStudent", new { id = student.Id }, mapper.Map<StudentDTO>(student));

        }

        private async Task<IdentityUser> CreateUser(string mail, string password)
        /*Method to create Users in Identity Framework Tables
            -AspNetRoles: to create user roles as Admin, Teacher or Student
            -AspNetUsers: to create users
            -AspNetUserClaims: to create a user claim to save password salt
            -AspnetUserRole: to asign a role to a user
         */
        {
            try
            {

                IdentityUser user = new IdentityUser()
                //First, create an IdentityUser instance
                {
                    UserName = mail,
                    Email = mail
                };

                var passwordHash = hashService.Hash(password);//Making a hash for password

                var roleExist = await roleManager.RoleExistsAsync(role);

                //Checkin if a role exist
                if (!roleExist)
                {
                    //If doesnt, then create one 
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

                await userManager.CreateAsync(user, passwordHash.Hash);//Create a user

                await userManager.AddToRoleAsync(user, role);//Assigning role to the user

                await userManager.AddClaimAsync(user, new Claim("Salt", Convert.ToBase64String(passwordHash.Salt)));//Saving salt from password hash


                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet("get/{id:int}", Name = "GetStudent")]
        public async Task<IActionResult> GetStudent(int id)
        {
            //Variable declares
            var student = new Student();
            var studentDTO = new StudentDTO();
            try
            {
                //Getting student and user info
                student = await context.Students
                    .Include(studentDB => studentDB.User)
                    .FirstOrDefaultAsync(studentDB => studentDB.Id == id);

                if (student == null) 
                {
                    return NotFound();
                }

                studentDTO = mapper.Map<StudentDTO>(student);
            }
            catch (Exception ex)
            {
                //Just in case, to not retorn server error
                return StatusCode(500, "Something goes wrong getting student data");
            }

            return Ok(studentDTO);
        }
        
        [HttpPatch("patch/{id:int}")]
        public async Task<IActionResult> PatchStudent(int id, [FromBody] JsonPatchDocument<StudentPatchDTO> patchDocument)
        {
            try
            {

                if (patchDocument == null) //Checking if doc isnt null
                {
                    return BadRequest();
                }

                //Getting original data from student
                var student = await context.Students
                    .Include(studentDB => studentDB.User)
                    .FirstOrDefaultAsync(studentDB => studentDB.Id == id);

                if (student == null) // Checking if client provides a real student id
                {
                    return NotFound();
                }

                var studentPatchDTO = mapper.Map<StudentPatchDTO>(student);

                patchDocument.ApplyTo(studentPatchDTO, ModelState);//Applying patch document modified data to DTO

                if (!TryValidateModel(studentPatchDTO))
                {
                    if (ModelState.ContainsKey("password")
                            && studentPatchDTO.password == student.User.PasswordHash)
                    //Password validation could execute needlessly
                    {
                        ModelState.Remove("password");
                    }

                    if (ModelState.ErrorCount > 0)
                    {
                        return BadRequest(ModelState);
                    }
                }

                if (studentPatchDTO.mail != student.User.Email) //Only if mail is changed 
                {

                    if (await userManager.FindByEmailAsync(studentPatchDTO.mail) != null)
                    {
                        return BadRequest("Mail address is not available");
                    }
                }

                var passwordIsChanged = studentPatchDTO.password != student.User.PasswordHash;
                //Just if password has changed
                if (passwordIsChanged)
                {
                    //First, we get our current data
                    var user = student.User;
                    var oldClaims = await userManager.GetClaimsAsync(user);
                    var saltClaim = oldClaims.FirstOrDefault(claimDB => claimDB.Type == "Salt");

                    var oldPasswordHash = hashService.Hash(studentPatchDTO.password, Convert.FromBase64String(saltClaim.Value));
                    //Lets check if the password that client send is diferent from the current one
                    if (oldPasswordHash.Hash == student.User.PasswordHash)
                    {
                        return BadRequest("Password must be diferent of current one");
                    }

                    //If it doesnt, lets create hash from new password
                    var newPasswordHash = hashService.Hash(studentPatchDTO.password);

                    //Replacing Salt from previus hash for the new one
                    var claimUpdate = await userManager.ReplaceClaimAsync(user, saltClaim
                        , new Claim("Salt", Convert.ToBase64String(newPasswordHash.Salt)));

                    if (!claimUpdate.Succeeded)
                    {
                        return StatusCode(500, "Something goes wrong updating user data");
                    }

                    studentPatchDTO.password = newPasswordHash.Hash;
                }

                mapper.Map(studentPatchDTO, student);

                student.User.NormalizedEmail = student.User.Email.ToUpper();
                student.User.UserName = student.User.Email.ToUpper();
                student.User.NormalizedUserName = student.User.Email.ToUpper();

                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something goes wrong updating student");
            }

            return NoContent();
        }
        
    }
}
