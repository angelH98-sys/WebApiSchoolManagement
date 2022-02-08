using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using WebApiSchoolManagement.DTO;
using WebApiSchoolManagement.DTO.TeacherDTOs;
using WebApiSchoolManagement.Models;
using WebApiSchoolManagement.Utilities;

namespace WebApiSchoolManagement.Controllers
{
    [ApiController]
    [Route("api/teachers")]
    public class TeachersController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IDataProtectionProvider dataProtectionProvider;
        private readonly HashService hashService;
        private readonly RoleManager<IdentityRole> roleManager;
        private string role;

        public TeachersController(
            ApplicationDBContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService,
            RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.dataProtectionProvider = dataProtectionProvider;
            this.hashService = hashService;
            this.roleManager = roleManager;
            this.role = "Teacher";
        }

        [HttpPost("create/admin")]
        public async Task<IActionResult> CreateTeacherAdmin([FromBody] TeacherCreationDTO teacherCreationDTO) 
        {
        
            var existAdmin = await roleManager.FindByNameAsync("Admin");
            //If admin role dont even exist, means that an Admin User niether
            if (existAdmin == null) 
            {
                role = "Admin";
                return await CreateTeacher(teacherCreationDTO);
            }

            var isAdminRoleAlreadyTaken = await userManager.GetUsersInRoleAsync("Admin");
            //If admin role exist, lets check if exist an admin user
            if (isAdminRoleAlreadyTaken.Count > 0) 
            {
                return BadRequest("Admin user already exist");
            }

            return await CreateTeacher(teacherCreationDTO);
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateTeacher([FromBody] TeacherCreationDTO teacherCreationDTO)
        {
            try
            {
                //First, execite teacher map
                Teacher teacher = mapper.Map<Teacher>(teacherCreationDTO);

                var username = "";
                do//Checking if username is available
                {
                    //Generating an username
                    username = UsernameGenerator.Genarate(teacher.name);

                } while (await userManager.FindByNameAsync(username) != null);

                //Checking if mail is available
                var isMailAvailable = await userManager.FindByEmailAsync(teacherCreationDTO.mail);
                if (isMailAvailable != null) 
                {
                    return BadRequest("Mail address is not available");
                }

                var user = await CreateUser(username, teacherCreationDTO.mail, teacherCreationDTO.password);

                if (user.Value == null)
                {
                    return StatusCode(500, "Something goes wrong on user creation");
                }

                teacher.UserId = user.Value.Id;

                context.Teachers.Add(teacher);

                await context.SaveChangesAsync();

                //Setting teacherDTO to return it

                teacher.User = user.Value;

                var teacherDTO = mapper.Map<TeacherDetailedDTO>(teacher);

                teacherDTO.role = role;

                return CreatedAtRoute("GetTeacher", new { id = teacher.Id }, teacherDTO);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, "Something goes wrong on teacher creation");
            }

        }
        private async Task<ActionResult<IdentityUser>> CreateUser(string username, string mail, string password)
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
                    UserName = username,
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

        [HttpGet("get/{id:int}", Name = "GetTeacher")]
        public async Task<ActionResult<TeacherDetailedDTO>> GetTeacher(int id)
        {
            try
            {

                var teacher = await context.Teachers.FindAsync(id);
                //First, we get teacher data
                if (teacher == null)
                {
                    return NotFound();
                }

                teacher.User = await userManager.FindByIdAsync(teacher.UserId);
                //Then, we get user data from identity tables
                if (teacher.User == null)
                {
                    return StatusCode(500, "Something goes wrong getting teacher");
                }

                var teacherDetailedDTO = mapper.Map<TeacherDetailedDTO>(teacher);

                //To get user role
                var roleList = await userManager.GetRolesAsync(teacher.User);

                if (roleList == null || roleList.Count == 0)
                {
                    teacherDetailedDTO.role = "";
                }
                else
                {
                    teacherDetailedDTO.role = roleList[0];
                }

                return teacherDetailedDTO;
            }
            catch (Exception ex) 
            {
                return StatusCode(500, "Something goes wrong with getting teacher data");
            }
        }

        [HttpGet("get/all")]
        public async Task<List<TeacherDTO>> GetAllTeachers()
        {
            var teachers = await context.Teachers
                .Include(teacherDB => teacherDB.User)
                .ToListAsync();

            var teacherDTOList = new List<TeacherDTO>();

            foreach (var teacher in teachers)
                teacherDTOList.Add(mapper.Map<TeacherDTO>(teacher));

            return teacherDTOList;
        }

        [HttpPatch("patch/{id:int}")]
        public async Task<IActionResult> PatchTeacher(int id, JsonPatchDocument<TeacherPatchDTO> patchDocument)
        {
            try
            {
                if (patchDocument == null)//Checking if patch document is missing 
                {
                    return BadRequest();
                }

                var teacher = await context.Teachers.FirstOrDefaultAsync(teacherDB => teacherDB.Id == id);

                if (teacher == null) // Checking if client provides a real teacher id
                {
                    return NotFound();
                }

                teacher.User = await userManager.FindByIdAsync(teacher.UserId);

                if (teacher.User == null) //Just in case, if user doesnt exist
                {
                    return StatusCode(500, "Something goes wrong finding teacher data");
                }

                var teacherPatchDTO = mapper.Map<TeacherPatchDTO>(teacher);

                patchDocument.ApplyTo(teacherPatchDTO, ModelState);//Applying patch document modified data to DTO

                if (!TryValidateModel(teacherPatchDTO))
                {
                    if (ModelState.ContainsKey("password")
                        && teacherPatchDTO.password == teacher.User.PasswordHash)
                    //Password validation could execute needlessly
                    {
                        ModelState.Remove("password");
                    }

                    if (ModelState.ErrorCount > 0) 
                    {
                        return BadRequest(ModelState);
                    }
                }

                if (teacherPatchDTO.mail != teacher.User.Email) //Only if mail has changed 
                {

                    var isMailAvailable = await userManager.FindByEmailAsync(teacherPatchDTO.mail);
                    if (isMailAvailable != null)
                    {
                        return BadRequest("Mail address is not available");
                    }
                }

                var passwordIsChanged = teacherPatchDTO.password != teacher.User.PasswordHash;
                //Just if password has changed
                if (passwordIsChanged) 
                {
                    //First, we get our current data
                    var user = teacher.User;
                    var oldClaims = await userManager.GetClaimsAsync(user);
                    var saltClaim = oldClaims.FirstOrDefault(claimDB => claimDB.Type == "Salt");

                    var oldPasswordHash = hashService.Hash(teacherPatchDTO.password, Convert.FromBase64String(saltClaim.Value));
                    //Lets check if the password that client send is diferent from the current one
                    if (oldPasswordHash.Hash == teacher.User.PasswordHash) 
                    {
                        return BadRequest("Password must be diferent of current one");
                    }

                    //If it doesnt, lets create hash from new password
                    var newPasswordHash = hashService.Hash(teacherPatchDTO.password);

                    //Replacing Salt from previus hash for the new one
                    var claimUpdate = await userManager.ReplaceClaimAsync(user, saltClaim
                        , new Claim("Salt", Convert.ToBase64String(newPasswordHash.Salt)));

                    if (!claimUpdate.Succeeded) 
                    {
                        return StatusCode(500, "Something goes wrong updating user data");
                    }

                    teacherPatchDTO.password = newPasswordHash.Hash;
                }

                mapper.Map(teacherPatchDTO, teacher);
            
                await context.SaveChangesAsync();
            }   
            catch (Exception ex)
            {
                return StatusCode(500, "Something goes wrong executing update");
            }

            return NoContent();
        }

    }
}
