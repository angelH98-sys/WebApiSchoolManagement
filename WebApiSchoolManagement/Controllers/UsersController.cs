using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiSchoolManagement.DTO;
using WebApiSchoolManagement.DTO.UserDTOs;
using WebApiSchoolManagement.Models;
using WebApiSchoolManagement.Utilities;

namespace WebApiSchoolManagement.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly ApplicationDBContext context;

        public UsersController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            HashService hashService,
            ApplicationDBContext context)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            this.context = context;
        }


        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LogInDTO logInDTO)
        {

            var user = await this.userManager.FindByEmailAsync(logInDTO.Email);

            if (user == null)
            {
                return BadRequest("Failed login");
            }

            var userClaims = await userManager.GetClaimsAsync(user);

            if (userClaims == null || userClaims.Count == 0) 
            {
                return StatusCode(500, "Something goes wrong on login");
            }

            var salt = userClaims.Where(claim => claim.Type == "Salt").FirstOrDefault();

            if (salt == null) 
            {
                return StatusCode(500, "Something goes wrong on login");
            }

            var userRoles = await userManager.GetRolesAsync(user);

            if (userRoles.FirstOrDefault() == null) 
            {
                return StatusCode(500, "Something goes wrong on login");
            }

            var passwordHash = hashService.Hash(logInDTO.Password, Convert.FromBase64String(salt.Value));

            var resultado = await signInManager.PasswordSignInAsync(user.UserName, passwordHash.Hash,
                isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return ConstruirToken(logInDTO, userRoles.FirstOrDefault(), user.Email);
            }
            else
            {
                return BadRequest("Failed login");
            }
        }
        private string ConstruirToken(LogInDTO logInDTO, string role, string mail)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim("mail", mail));

            switch (role) 
            {
                case "Admin":
                    claims.Add(new Claim("Admin", "1"));
                    break;
                case "Student":
                    claims.Add(new Claim("Student", "1"));
                    break;
                case "Teacher":
                    claims.Add(new Claim("Teacher", "1"));
                    break;
            }

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwtkey"]));

            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(10);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims,
                expires: expiration, signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}
